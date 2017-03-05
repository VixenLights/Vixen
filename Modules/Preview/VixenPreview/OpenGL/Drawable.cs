using System;
using OpenTK;

namespace VixenModules.Preview.VixenPreview.OpenGL
{
	public interface IDrawable:IDisposable
	{
		void Draw(Matrix4 fov, Matrix4 cameraView);
	}
}