using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheSaga.Models.History;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga
{
    internal static class Middlewares
    {
        static List<Type> beforeSetMiddlewares = new List<Type>();
        static List<Type> afterGetMiddlewares = new List<Type>();

        public static void AddBeforeMiddlewares(Type type) =>
            beforeSetMiddlewares.Add(type);

        public static void AddAfterMiddlewares(Type type) =>
            afterGetMiddlewares.Add(type);

        /* public static async Task InvokeChain(
           IServiceProvider serviceProvider,
            ISaga saga,
            ISagaStep sagaStep,
            StepData stepData,
            params ExecuteSagaDelegate[] executeDelegates)
        {
            List<NextMiddleware> actions = null;
            try
            {
                actions = BuildChain(serviceProvider, executeDelegates);
                await actions[0].Invoke(saga, sagaStep, stepData);
            }
            finally
            {
                if (actions != null)
                {
                    foreach (var action in actions)
                        action.Clean();
                    actions.Clear();
                }
            }
        }*/
        public static async Task ExecuteChain(
            List<NextMiddleware> middlewaresChain,
            ISaga saga,
            ISagaStep sagaStep,
            StepData stepData)
        {
            await middlewaresChain[0].Invoke(saga, sagaStep, stepData);
        }

        public static MiddlewaresChain BuildChain(IServiceProvider serviceProvider, params ExecuteSagaDelegate[] executeDelegates)
        {
            MiddlewaresChain actions = new MiddlewaresChain();
            foreach (var item in beforeSetMiddlewares)
                actions.Add(new NextMiddleware(serviceProvider, item, null));

            foreach (var executeDelegate in executeDelegates)
                actions.Add(new NextMiddleware(serviceProvider, null, executeDelegate));

            foreach (var item in afterGetMiddlewares)
                actions.Add(new NextMiddleware(serviceProvider, item, null));

            for (var i = 0; i < actions.Count - 1; i++)
            {
                var action = actions[i];
                var nextAction = actions[i + 1];
                action.Next = nextAction;
            }

            return actions;
        }
    }

    public static class ListHelper
    {
        public static T NextAfter<T>(this ICollection<T> collection, T item)
        {
            bool found = false;
            foreach (var i in collection)
            {
                if (found)
                {
                    return i;
                }
                else if ((i == null && item == null) || (i != null && i.Equals(item)))
                {
                    found = true;
                }
            }
            return default(T);
        }
    }

    public delegate Task ExecuteSagaDelegate(ISaga saga, ISagaStep sagaStep, StepData stepData);

    internal class MiddlewaresChain : List<NextMiddleware>
    {
        public void Clean()
        {
            foreach (var item in this)
                item.Clean();
        }
    }
    internal class NextMiddleware : INextMiddleware
    {
        public NextMiddleware(IServiceProvider serviceProvider, Type middlewareType, ExecuteSagaDelegate middlewareAction)
        {
            ServiceProvider = serviceProvider;
            MiddlewareType = middlewareType;
            MiddlewareAction = middlewareAction;
        }

        public IServiceProvider ServiceProvider { get; set; }
        public Type MiddlewareType { get; set; }
        public ExecuteSagaDelegate MiddlewareAction { get; set; }
        public NextMiddleware Next { get; set; }

        public async Task Invoke(
            ISaga saga,
            ISagaStep sagaStep,
            StepData stepData)
        {
            if (MiddlewareAction != null)
            {
                await MiddlewareAction(saga, sagaStep, stepData);
                if (Next != null)
                    await Next.Invoke(saga, sagaStep, stepData);
            }
            else
            {
                var middleware = ActivatorUtilities.CreateInstance(ServiceProvider, MiddlewareType) as ISagaMiddleware;
                await middleware.InvokeAsync(saga, sagaStep, stepData, Next);
            }
        }

        internal void Clean()
        {
            ServiceProvider = null;
            MiddlewareType = null;
            MiddlewareAction = null;
            Next = null;
        }
    }

    public interface INextMiddleware
    {
        Task Invoke(ISaga saga, ISagaStep sagaStep, StepData stepData);
    }

    public interface ISagaMiddleware
    {
        Task InvokeAsync(ISaga saga, ISagaStep sagaStep, StepData stepData, INextMiddleware nextMiddleware);
    }
}
