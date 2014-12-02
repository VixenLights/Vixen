using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.Module.Effect
{
	[Serializable]
	public abstract class EffectModuleInstanceBase : ModuleInstanceBase, IEffectModuleInstance,
	                                                 IEqualityComparer<IEffectModuleInstance>,
	                                                 IEquatable<IEffectModuleInstance>,
	                                                 IEqualityComparer<EffectModuleInstanceBase>,
	                                                 IEquatable<EffectModuleInstanceBase>
	{
		private ElementNode[] _targetNodes;
		private TimeSpan _timeSpan;
		private DefaultValueArrayMember _parameterValues;
		protected ElementIntents _elementIntents;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		protected EffectModuleInstanceBase()
		{
			_targetNodes = new ElementNode[0]; //set member directly on creation to prevent target node changed events from occuring.
			TimeSpan = TimeSpan.Zero;
			IsDirty = true;
			_parameterValues = new DefaultValueArrayMember(this);
			_elementIntents = new ElementIntents();
		}

		public virtual bool IsDirty { get; protected set; }
		private bool IsRendering;

		public ElementNode[] TargetNodes
		{
			get { return _targetNodes; }
			set
			{
				if (value != _targetNodes) {
					_targetNodes = value;
					_EnsureTargetNodeProperties();
					CalculateAffectedElements();
					TargetNodesChanged();
					IsDirty = true;
				}
			}
		}

		public IEnumerable<Guid> EffectedElementIds { get; set; }

		public TimeSpan TimeSpan
		{
			get { return _timeSpan; }
			set
			{
				if (value != _timeSpan) {
					_timeSpan = value;
					IsDirty = true;
				}
			}
		}

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
				PreRender();
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

		public virtual string EffectName
		{
			get { return ((IEffectModuleDescriptor) Descriptor).EffectName; }
		}

		public EffectGroups EffectGroup
		{
			get { return ((IEffectModuleDescriptor)Descriptor).EffectGroup; }
		}

		public ParameterSignature Parameters
		{
			get { return ((IEffectModuleDescriptor) Descriptor).Parameters; }
		}

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


			using (Font AdjustedFont =  Vixen.Common.Graphics.GetAdjustedFont(g, DisplayValue, clipRectangle, "Arial")) {
				using (var StringBrush = new SolidBrush(Color.Black)) {
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
			foreach (Guid elementId in effectIntents.ElementIds) {
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

			if (!ApplicationServices.AreAllEffectRequiredPropertiesPresent(this)) {
				EffectModuleDescriptorBase effectDescriptor =
					Modules.GetDescriptorById<EffectModuleDescriptorBase>(Descriptor.TypeId);
				using (var ms = new MemoryStream()) {
					using (var sw = new StreamWriter(ms)) {
						sw.WriteLine("The \"{0}\" effect has property requirements that are missing:\n", effectDescriptor.TypeName);

						foreach (ElementNode elementNode in TargetNodes) {
							Guid[] missingPropertyIds =
								effectDescriptor.PropertyDependencies.Except(elementNode.Properties.Select(x => x.Descriptor.TypeId)).ToArray();
							if (missingPropertyIds.Length > 0) {
								sw.WriteLine((elementNode.Children.Any() ? "Group " : "Element ") + elementNode.Name);
								missingPropertyIds.Select(x => string.Format(" - Property {0}", Modules.GetDescriptorById(x).TypeName)).ToList()
									.ForEach(p => { sw.WriteLine(p); });
							}
						}
						sw.Flush();
						ms.Position = 0;
						using (var sr = new StreamReader(ms)) {
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

		public virtual bool ForceGenerateVisualRepresentation
		{
			get { return false; }
		}

		#endregion
	}
}