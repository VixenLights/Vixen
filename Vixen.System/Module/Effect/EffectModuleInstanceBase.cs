using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Vixen.Annotations;
using Vixen.Attributes;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.Module.Effect
{
	[Serializable]
	public abstract class EffectModuleInstanceBase : ModuleInstanceBase, IEffectModuleInstance,
		IEqualityComparer<IEffectModuleInstance>,
		IEquatable<IEffectModuleInstance>,
		IEqualityComparer<EffectModuleInstanceBase>,
		IEquatable<EffectModuleInstanceBase>,
		INotifyPropertyChanged, ICustomTypeDescriptor
	{
		private ElementNode[] _targetNodes;
		private TimeSpan _timeSpan;
		private DefaultValueArrayMember _parameterValues;
		protected ElementIntents _elementIntents;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private readonly Dictionary<string, bool> _browsableState = new Dictionary<string, bool>();

		protected EffectModuleInstanceBase()
		{
			_targetNodes = new ElementNode[0];
				//set member directly on creation to prevent target node changed events from occuring.
			TimeSpan = TimeSpan.Zero;
			IsDirty = true;
			_parameterValues = new DefaultValueArrayMember(this);
			_elementIntents = new ElementIntents();
		}

		[Browsable(false)]
		public virtual bool IsDirty { get; protected set; }

		private bool IsRendering;

		[Browsable(false)]
		public ElementNode[] TargetNodes
		{
			get { return _targetNodes; }
			set
			{
				if (value != _targetNodes)
				{
					_targetNodes = value;
					_EnsureTargetNodeProperties();
					CalculateAffectedElements();
					TargetNodesChanged();
					IsDirty = true;
				}
			}
		}

		[Browsable(false)]
		public IEnumerable<Guid> EffectedElementIds { get; set; }

		[Browsable(false)]
		public TimeSpan TimeSpan
		{
			get { return _timeSpan; }
			set
			{
				if (value != _timeSpan)
				{
					_timeSpan = value;
					IsDirty = true;
				}
			}
		}

		[Browsable(false)]
		public object[] ParameterValues
		{
			get { return _parameterValues.Values; }
			set
			{
				_parameterValues.Values = value;
				IsDirty = true;
			}
		}

		public void PreRender(CancellationTokenSource cancellationToken = null)
		{
			_PreRender();
			IsDirty = false;
		}

		public EffectIntents Render()
		{
			if (IsDirty && !IsRendering)
			{
				IsRendering = true;
				try
				{
					PreRender();
				}
				catch (Exception e)
				{
					//Trap any errors to prevent the effect from staying in a state of rendering.
					Logging.Error(String.Format("Error rendering {0}", EffectName), e);
				}

				IsRendering = false;
			}
			else
			{
				//To prevent the effect from being rendered multiple times if multiple threads 
				//try to access it all at the same time. I.E the editor pre rendering process.
				while (IsRendering)
				{
					Thread.Sleep(1);
				}
			}

			return _Render();
		}

		public EffectIntents Render(TimeSpan restrictingOffsetTime, TimeSpan restrictingTimeSpan)
		{
			EffectIntents effectIntents = Render();
			// NB: the ElementData.Restrict method takes a start and end time, not a start and duration
			effectIntents = EffectIntents.Restrict(effectIntents, restrictingOffsetTime,
				restrictingOffsetTime + restrictingTimeSpan);
			return effectIntents;
		}

		/// <summary>
		/// This is called when the elements change to give the effect a chance to process any properties
		/// or validate anything like colors, etc
		/// </summary>
		protected abstract void TargetNodesChanged();

		protected abstract void _PreRender(CancellationTokenSource cancellationToken = null);

		protected abstract EffectIntents _Render();

		[DisplayName(@"Effect Name")]
		[Category(@"Effect")]
		[PropertyOrder(1)]
		public virtual string EffectName
		{
			get { return Descriptor != null ? ((IEffectModuleDescriptor) Descriptor).EffectName : ""; }
		}

		[DisplayName(@"Effect Group")]
		[Category(@"Effect")]
		[PropertyOrder(2)]
		public EffectGroups EffectGroup
		{
			get { return ((IEffectModuleDescriptor) Descriptor).EffectGroup; }
		}

		[Browsable(false)]
		public ParameterSignature Parameters
		{
			get { return ((IEffectModuleDescriptor) Descriptor).Parameters; }
		}

		[Browsable(false)]
		public Guid[] PropertyDependencies
		{
			get { return ((EffectModuleDescriptorBase) Descriptor).PropertyDependencies; }
		}

		//public virtual void GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle)
		//{
		//	g.Clear(Color.White);
		//	g.DrawRectangle(Pens.Black, clipRectangle.X, clipRectangle.Y, clipRectangle.Width - 1, clipRectangle.Height - 1);
		//}
		public virtual void GenerateVisualRepresentation(System.Drawing.Graphics g, System.Drawing.Rectangle clipRectangle)
		{

			string DisplayValue = string.Format("{0}", this.EffectName);


			using (Font AdjustedFont = Vixen.Common.Graphics.GetAdjustedFont(g, DisplayValue, clipRectangle, "Arial"))
			{
				using (var StringBrush = new SolidBrush(Color.Black))
				{
					//g.Clear(Color.White);
					g.DrawRectangle(Pens.Black, clipRectangle.X, clipRectangle.Y, clipRectangle.Width - 1, clipRectangle.Height - 1);

					g.DrawString(DisplayValue, AdjustedFont, StringBrush, 4, 4);
					//base.GenerateVisualRepresentation(g, clipRectangle);
				}
			}
		}

		public virtual ElementIntents GetElementIntents(TimeSpan effectRelativeTime)
		{
			_elementIntents.Clear();

			_AddLocalIntents(effectRelativeTime);

			return _elementIntents;
		}

		private void _AddLocalIntents(TimeSpan effectRelativeTime)
		{
			EffectIntents effectIntents = Render();
			foreach (Guid elementId in effectIntents.ElementIds)
			{
				IIntentNode[] elementIntents = effectIntents.GetElementIntentsAtTime(elementId, effectRelativeTime);
				_elementIntents.AddIntentNodeToElement(elementId, elementIntents);
			}
		}

		private void CalculateAffectedElements()
		{
			EffectedElementIds =
				TargetNodes.SelectMany(y => y.GetElementEnumerator()).Select(z => z.Id);
		}

		private void _EnsureTargetNodeProperties()
		{
			// If the effect requires any properties, make sure the target nodes have those properties.
			if (TargetNodes == null || TargetNodes.Length == 0)
				return;

			if (!ApplicationServices.AreAllEffectRequiredPropertiesPresent(this))
			{
				EffectModuleDescriptorBase effectDescriptor =
					Modules.GetDescriptorById<EffectModuleDescriptorBase>(Descriptor.TypeId);
				using (var ms = new MemoryStream())
				{
					using (var sw = new StreamWriter(ms))
					{
						sw.WriteLine("The \"{0}\" effect has property requirements that are missing:\n", effectDescriptor.TypeName);

						foreach (ElementNode elementNode in TargetNodes)
						{
							Guid[] missingPropertyIds =
								effectDescriptor.PropertyDependencies.Except(elementNode.Properties.Select(x => x.Descriptor.TypeId)).ToArray();
							if (missingPropertyIds.Length > 0)
							{
								sw.WriteLine((elementNode.Children.Any() ? "Group " : "Element ") + elementNode.Name);
								missingPropertyIds.Select(x => string.Format(" - Property {0}", Modules.GetDescriptorById(x).TypeName)).ToList()
									.ForEach(p => { sw.WriteLine(p); });
							}
						}
						sw.Flush();
						ms.Position = 0;
						using (var sr = new StreamReader(ms))
						{
							throw new InvalidOperationException(sr.ReadToEnd());
						}
						// throw new InvalidOperationException(string.Join(Environment.NewLine, message));
					}
				}
			}
		}

		public override string ToString()
		{
			return EffectName;
		}

		public bool Equals(IEffectModuleInstance x, IEffectModuleInstance y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IEffectModuleInstance obj)
		{
			return base.GetHashCode(obj);
		}

		public bool Equals(IEffectModuleInstance other)
		{
			return base.Equals(other);
		}

		public bool Equals(EffectModuleInstanceBase x, EffectModuleInstanceBase y)
		{
			return Equals(x as IEffectModuleInstance, y as IEffectModuleInstance);
		}

		public int GetHashCode(EffectModuleInstanceBase obj)
		{
			return GetHashCode(obj as IEffectModuleInstance);
		}

		public bool Equals(EffectModuleInstanceBase other)
		{
			return Equals(other as IEffectModuleInstance);
		}

		#region IEffectModuleInstance Members

		[Browsable(false)]
		public virtual bool ForceGenerateVisualRepresentation
		{
			get { return false; }
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region ICustomTypeDescriptor

		public virtual AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public virtual string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public virtual string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public virtual TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public virtual EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public virtual PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public virtual EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetPropertiesImpl(null);
		}

		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return GetPropertiesImpl(attributes);

		}

		public virtual PropertyDescriptorCollection GetPropertiesImpl(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(this, attributes, true);
			//Enhance the base properties with our updated browsable attributes
			lock (_browsableState)
			{
				var t = propertyDescriptorCollection.Cast<PropertyDescriptor>().Select(prop =>
				{
					if (_browsableState.ContainsKey(prop.Name))
					{
						bool state = _browsableState[prop.Name];
						List<Attribute> newAttributes =
							prop.Attributes.Cast<Attribute>().Where(attribute => !(attribute is BrowsableAttribute)).ToList();
						newAttributes.Add(new BrowsableAttribute(state));
						return TypeDescriptor.CreateProperty(GetType(), prop, newAttributes.ToArray());
					}

					return prop;

				});

				return new PropertyDescriptorCollection(t.ToArray());
			}

		}

		public virtual object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public void SetBrowsable(string property, bool browsable)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1) {{property, browsable}};
			SetBrowsable(propertyStates);
		}

		public void SetBrowsable(Dictionary<string, bool> propertyStates)
		{
			lock (_browsableState)
			{
				foreach (var propertyState in propertyStates)
				{
					if (_browsableState.ContainsKey(propertyState.Key))
					{
						_browsableState[propertyState.Key] = propertyState.Value;
					}
					else
					{
						_browsableState.Add(propertyState.Key, propertyState.Value);
					}
				}

			}

		}


		#endregion
	}

}
