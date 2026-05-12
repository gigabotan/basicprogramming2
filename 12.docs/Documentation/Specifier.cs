using System;
using System.Linq;
using System.Reflection;

namespace Documentation;

public class Specifier<T> : ISpecifier
{
	public string GetApiDescription()
	{
		return GetDescription(typeof(T));
	}

	public string[] GetApiMethodNames()
	{
		return GetApiMethods()
			.Where(method => method.GetCustomAttribute<ApiDescriptionAttribute>() != null)
			.Select(method => method.Name)
			.ToArray();
	}

	public string GetApiMethodDescription(string methodName)
	{
		return GetDescription(GetApiMethod(methodName));
	}

	public string[] GetApiMethodParamNames(string methodName)
	{
		return GetApiMethod(methodName)
			?.GetParameters()
			.Select(parameter => parameter.Name)
			.ToArray();
	}

	public string GetApiMethodParamDescription(string methodName, string paramName)
	{
		return GetDescription(GetParameter(methodName, paramName));
	}

	public ApiParamDescription GetApiMethodParamFullDescription(string methodName, string paramName)
	{
		return CreateParamDescription(GetParameter(methodName, paramName), paramName);
	}

	public ApiMethodDescription GetApiMethodFullDescription(string methodName)
	{
		var method = GetApiMethod(methodName);
		if (method == null)
			return null;

		return new ApiMethodDescription
		{
			MethodDescription = new CommonDescription(method.Name, GetDescription(method)),
			ParamDescriptions = method
				.GetParameters()
				.Select(parameter => CreateParamDescription(parameter, parameter.Name))
				.ToArray(),
			ReturnDescription = CreateReturnDescription(method)
		};
	}

	private static MethodInfo[] GetApiMethods()
	{
		return typeof(T)
			.GetMethods(BindingFlags.Instance | BindingFlags.Public)
			.Where(method => method.GetCustomAttribute<ApiMethodAttribute>() != null)
			.ToArray();
	}

	private static MethodInfo GetApiMethod(string methodName)
	{
		return GetApiMethods().FirstOrDefault(method => method.Name == methodName);
	}

	private static ParameterInfo GetParameter(string methodName, string paramName)
	{
		return GetApiMethod(methodName)
			?.GetParameters()
			.FirstOrDefault(parameter => parameter.Name == paramName);
	}

	private static string GetDescription(MemberInfo member)
	{
		return member?.GetCustomAttribute<ApiDescriptionAttribute>()?.Description;
	}

	private static string GetDescription(ParameterInfo parameter)
	{
		return parameter?.GetCustomAttribute<ApiDescriptionAttribute>()?.Description;
	}

	private static ApiParamDescription CreateParamDescription(ParameterInfo parameter, string paramName)
	{
		var validation = parameter?.GetCustomAttribute<ApiIntValidationAttribute>();
		var required = parameter?.GetCustomAttribute<ApiRequiredAttribute>();

		return new ApiParamDescription
		{
			ParamDescription = new CommonDescription(paramName, GetDescription(parameter)),
			Required = required?.Required ?? false,
			MinValue = validation?.MinValue,
			MaxValue = validation?.MaxValue
		};
	}

	private static ApiParamDescription CreateReturnDescription(MethodInfo method)
	{
		var returnParameter = method.ReturnParameter;
		var hasDescription = returnParameter.GetCustomAttribute<ApiDescriptionAttribute>() != null;
		var hasRequired = returnParameter.GetCustomAttribute<ApiRequiredAttribute>() != null;
		var hasValidation = returnParameter.GetCustomAttribute<ApiIntValidationAttribute>() != null;

		if (!hasDescription && !hasRequired && !hasValidation)
			return null;

		return CreateParamDescription(returnParameter, null);
	}
}
