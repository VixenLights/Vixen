using System;
using System.IO;
using System.Xml;
using System.ComponentModel;
using CommonElements.ColorManagement.ColorModels;
using CommonElements.ControlsEx;

namespace VixenModules.App.ColorGradients
{
	/// <summary>
	/// collection class for gradients, supporting IO
	/// </summary>
	public class GradientCollection : CollectionBase<ColorGradient>
	{
		protected override void OnValidate(ColorGradient value)
		{
			if ((value as ColorGradient) == null)
				throw new ArgumentException("no gradient");
		}
		#region io methods
		/// <summary>
		/// loads gradients from a file and adds them
		/// </summary>
		public void Load(string filename)
		{
			if (filename == null)
				throw new ArgumentNullException("filename");
			//load from file
			FileStream fstr = null;
			try
			{
				fstr = new FileStream(filename, FileMode.Open);
				this.Load(fstr);
				fstr.Close();
			}
			catch (Exception e)
			{
				if (fstr != null)
					fstr.Close();
				throw e;
			}
		}
		/// <summary>
		/// loads gradients from a stream and adds them
		/// </summary>
		public void Load(Stream str)
		{
			new XmlFormat().Load(this, str);
		}
		/// <summary>
		/// saves the collection using the default format
		/// </summary>
		public void Save(string filename)
		{
			this.Save(filename, null);
		}
		/// <summary>
		/// saves the collection to a file
		/// </summary>
		public void Save(string filename, Format format)
		{
			if (filename == null)
				throw new ArgumentNullException("filename");
			//save to file
			FileStream fstr = null;
			try
			{
				fstr = new FileStream(filename, FileMode.Create);
				this.Save(fstr,format);
				fstr.Flush();
				fstr.Close();
			}
			catch (Exception e)
			{
				if (fstr != null)
					fstr.Close();
				throw e;
			}
		}
		/// <summary>
		/// saves the collection to a stream
		/// </summary>
		public void Save(Stream str, Format formatter)
		{
			if (formatter == null)
				new XmlFormat().Write(this, str);
			else formatter.Write(this, str);
		}
		#endregion
	}
	/// <summary>
	/// prototype for formatting
	/// </summary>
	public interface Format
	{
		bool CanLoad(Stream str);
		void Write(GradientCollection value, Stream str);
		void Load(GradientCollection value, Stream str);
	}
	/// <summary>
	/// formatter for xml gradient list format
	/// </summary>
	public class XmlFormat : Format
	{
		/// <summary>
		/// writes an non-validated xml file
		/// </summary>
		public void Write(GradientCollection value, Stream str)
		{
			if (value == null || str == null)
				return;
			//
			TypeConverter xyzconv = TypeDescriptor.GetConverter(typeof(XYZ)),
				doubleconv = TypeDescriptor.GetConverter(typeof(double)),
				boolconv = TypeDescriptor.GetConverter(typeof(bool));
			//
			XmlDocument doc = new XmlDocument();
			XmlElement list = doc.CreateElement("Collection");
			doc.AppendChild(list);
			foreach (ColorGradient grd in value)
			{
				XmlElement elem = doc.CreateElement("Gradient");
				elem.SetAttribute("GammaCorrected", boolconv.ConvertToInvariantString(grd.Gammacorrected));
				elem.SetAttribute("Title", grd.Title.ToString());
				//write color points
				foreach (ColorPoint cp in grd.Colors)
				{
					XmlElement cpelem = doc.CreateElement("ColorPoint");
					cpelem.SetAttribute("Position", doubleconv.ConvertToInvariantString(cp.Position));
					cpelem.SetAttribute("Focus", doubleconv.ConvertToInvariantString(cp.Focus));
					cpelem.SetAttribute("Color", xyzconv.ConvertToInvariantString(cp.Color));
					elem.AppendChild(cpelem);
				}
				//write alpha points
				foreach (AlphaPoint cp in grd.Alphas)
				{
					XmlElement cpelem = doc.CreateElement("AlphaPoint");
					cpelem.SetAttribute("Position", doubleconv.ConvertToInvariantString(cp.Position));
					cpelem.SetAttribute("Focus", doubleconv.ConvertToInvariantString(cp.Focus));
					cpelem.SetAttribute("Alpha", doubleconv.ConvertToInvariantString(cp.Alpha));
					elem.AppendChild(cpelem);
				}
				list.AppendChild(elem);
			}
			doc.Save(str);
		}
		/// <summary>
		/// loads from a non-validated xml file
		/// </summary>
		public void Load(GradientCollection value, Stream str)
		{
			if (value == null || str == null)
				return;
			//
			TypeConverter xyzconv = TypeDescriptor.GetConverter(typeof(XYZ)),
				doubleconv = TypeDescriptor.GetConverter(typeof(double)),
				boolconv = TypeDescriptor.GetConverter(typeof(bool));
			//
			XmlDocument doc = new XmlDocument();
			doc.Load(str);
			XmlNodeList collections = doc.GetElementsByTagName("Collection");
			if (collections.Count < 1)
				throw new Exception("found no collection");
			foreach (XmlElement gradient in collections[0])
			{
				ColorGradient grd = new ColorGradient();
				grd.Title = gradient.GetAttribute("Title");
				grd.Gammacorrected = (bool)boolconv.ConvertFromInvariantString(gradient.GetAttribute("GammaCorrected"));
				//read color points
				foreach (XmlElement point in gradient)
				{
					if (point.Name == "ColorPoint")
						grd.Colors.Add(new ColorPoint(
							(XYZ)xyzconv.ConvertFromInvariantString(point.GetAttribute("Color")),
							(double)doubleconv.ConvertFromInvariantString(point.GetAttribute("Focus")),
							(double)doubleconv.ConvertFromInvariantString(point.GetAttribute("Position"))));
					else if (point.Name == "AlphaPoint")
						grd.Alphas.Add(new AlphaPoint(
							(double)doubleconv.ConvertFromInvariantString(point.GetAttribute("Alpha")),
							(double)doubleconv.ConvertFromInvariantString(point.GetAttribute("Focus")),
							(double)doubleconv.ConvertFromInvariantString(point.GetAttribute("Position"))));
					else
						throw new Exception("not a valid point type: "+point.Name);
				}
				value.Add(grd);
			}
		}

		public bool CanLoad(Stream str)
		{
			return true;
		}
	}
}
