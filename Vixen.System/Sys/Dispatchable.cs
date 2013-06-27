namespace Vixen.Sys
{
	//Contravariant - a concrete class may implement IHandler<SpecificClass> but the generic
	//                parameter at runtime may be IHandler<MoreBaseClass>.  In being
	//                contravariant, the runtime generic parameter can be of a less derived type.
	public interface IHandler<in HandledType>
	{
		void Handle(HandledType obj);
	}

	public interface IDispatchable
	{
		void Dispatch<HandlerType>(HandlerType handler);
	}

	public class Dispatchable<Dispatched> : IDispatchable
		where Dispatched : Dispatchable<Dispatched>
	{
		public void Dispatch<HandlerType>(HandlerType handler)
		{
			((IHandler<Dispatched>) handler).Handle((Dispatched) this);
		}
	}
}