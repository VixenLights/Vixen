using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;

namespace ControlsEx
{
	/// <summary>
	/// IPBox for typing of IPs
	/// </summary>
	public class IPBox:TextBox
	{
		private byte[] _address=new byte[]{192,168,1,1};
		public IPBox()
		{
			base.Text="   .   .   .   ";
			base.MaxLength=16;
		}
		#region helpers
		private bool TryParse(string s, out int number)
		{
			double res;
			bool b=double.TryParse(s,System.Globalization.NumberStyles.Integer,null,out res);
			number=(int)res;
			return b;
		}
		#endregion
		#region controller
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged (e);
			string[] frags=base.Text.Split('.');
			int bt, sel=base.SelectionStart;
			string txt;
			for (int i=0; i<Math.Min(4,frags.Length); i++)
			{
				if (TryParse(frags[i],out bt))
				{
					bt=Math.Max(0,Math.Min(255,bt));
					_address[i]=(byte)bt;
					frags[i]=bt.ToString();
					//frags[i]=frags[i].PadRight(3,' ');
				}
				else
				{
					frags[i]="   ";
					_address[i]=0;
				}
			}
			if (frags.Length>=4)
			{
				txt=string.Join(".",frags,0,4);
				if (base.Text!=txt)
					base.Text=txt;
			}
			else
			{
				txt=string.Join(".",frags,0,frags.Length);
				int i=frags.Length;
				while(i<4)
				{
					_address[i]=0;
					txt+=".   ";
					i++;
				}
				base.Text=txt;
					
			}
			base.SelectionStart=Math.Max(0,sel);
		}
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress (e);
			if(e.KeyChar=='.')
			{
				e.Handled=true;
				base.SelectionStart=Math.Max(0,Math.Min(base.TextLength-1,
					base.Text.IndexOf(".",base.SelectionStart)+1));
			}
			else if (!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
				e.Handled=true;
		}
		#endregion
		public byte[] IPAdress
		{
			get{return _address;}
		}
	}
}
