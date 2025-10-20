using OpenTK.Mathematics;

namespace Common.OpenGLCommon
{
	public interface IDrawable:IDisposable
	{
		void Draw(Matrix4 fov, Matrix4 cameraView);
	}
}