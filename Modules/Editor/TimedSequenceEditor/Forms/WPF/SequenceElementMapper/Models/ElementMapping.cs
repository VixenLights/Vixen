using System;
using System.Runtime.Serialization;
using Catel.Data;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Models
{
	public class ElementMapping: ModelBase
	{
		public ElementMapping(string sourceName)
		{
			SourceName = sourceName;
			TargetId = Guid.Empty;
			TargetName = String.Empty;
		}

		#region SourceName property

		/// <summary>
		/// Gets or sets the SourceName value.
		/// </summary>
		public string SourceName
		{
			get { return GetValue<string>(SourceNameProperty); }
			set { SetValue(SourceNameProperty, value); }
		}

		/// <summary>
		/// SourceName property data.
		/// </summary>
		public static readonly PropertyData SourceNameProperty = RegisterProperty("SourceName", typeof(string));

		#endregion

		#region TargetName property

		/// <summary>
		/// Gets or sets the TargetName value.
		/// </summary>
		public string TargetName
		{
			get { return GetValue<string>(TargetNameProperty); }
			set { SetValue(TargetNameProperty, value); }
		}

		/// <summary>
		/// TargetName property data.
		/// </summary>
		public static readonly PropertyData TargetNameProperty = RegisterProperty("TargetName", typeof(string));

		#endregion

		#region TargetId property

		/// <summary>
		/// Gets or sets the TargetId value.
		/// </summary>
		public Guid TargetId
		{
			get { return GetValue<Guid>(TargetIdProperty); }
			set { SetValue(TargetIdProperty, value); }
		}

		/// <summary>
		/// TargetId property data.
		/// </summary>
		public static readonly PropertyData TargetIdProperty = RegisterProperty("TargetId", typeof(Guid));

		#endregion



	}
}
