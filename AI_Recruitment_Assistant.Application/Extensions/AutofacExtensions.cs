using AI_Recruitment_Assistant.Application.Behaviors;
using Autofac;
using MediatR;
using System.Reflection;

namespace AI_Recruitment_Assistant.Application.Extensions;

public static class AutofacExtensions
{
    public static ContainerBuilder AddApplicationAutofacModules(this ContainerBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        builder.RegisterType<Mediator>()
            .As<IMediator>()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>))
            .InstancePerLifetimeScope();

        builder.RegisterGeneric(typeof(ExceptionHandlingBehavior<,>))
           .As(typeof(IPipelineBehavior<,>))
           .InstancePerLifetimeScope();

        builder.RegisterGeneric(typeof(UnitOfWorkBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();

        return builder;
    }
}
