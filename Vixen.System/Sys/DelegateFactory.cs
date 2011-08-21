using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Vixen.Sys {
	delegate object LateBoundMethod(object target, params object[] arguments);
	delegate void LateBoundProcedure(object target, params object[] arguments);

	static class DelegateFactory {
		static public T Create<T>(MethodInfo method) {
			Type returnType = typeof(T).GetMethod("Invoke").ReturnType;
			ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
			ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

			MethodCallExpression call = Expression.Call(
			  Expression.Convert(instanceParameter, method.DeclaringType),
			  method,
			  _CreateParameterExpressions(method, argumentsParameter));

			Expression<T> lambda = Expression.Lambda<T>(
			  Expression.Convert(call, returnType),
			  instanceParameter,
			  argumentsParameter);

			return lambda.Compile();
		}

		static private Expression[] _CreateParameterExpressions(MethodInfo method, Expression argumentsParameter) {
			return method.GetParameters().Select((parameter, index) =>
			  Expression.Convert(
				Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)), parameter.ParameterType)).ToArray();
		}
	}
}
