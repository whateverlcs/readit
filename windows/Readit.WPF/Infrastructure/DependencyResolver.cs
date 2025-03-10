using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.WPF.Infrastructure
{
    public static class DependencyResolver
    {
        private static SimpleContainer _container;

        // Configurar o contêiner
        public static void SetContainer(SimpleContainer container)
        {
            _container = container;
        }

        // Resolver um serviço pelo tipo
        public static T GetService<T>()
        {
            if (_container == null)
                throw new InvalidOperationException("Contêiner não configurado.");

            return (T)_container.GetInstance(typeof(T), null);
        }

        // Criar uma instância resolvendo dependências e permitindo parâmetros dinâmicos
        public static object CreateInstance(Type type, params object[] extraArgs)
        {
            if (_container == null)
                throw new InvalidOperationException("Contêiner não configurado.");

            var constructors = type.GetConstructors();

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var arguments = new List<object>();

                foreach (var param in parameters)
                {
                    // Se o tipo do parâmetro for igual ao de um argumento passado, usa o argumento
                    var argument = extraArgs.FirstOrDefault(a => param.ParameterType.IsAssignableFrom(a.GetType()));

                    if (argument != null)
                    {
                        arguments.Add(argument);
                    }
                    else
                    {
                        // Caso contrário, tenta resolver via DI
                        var service = _container.GetInstance(param.ParameterType, null);
                        if (service == null)
                            break; // Se não encontrar um serviço, passa para o próximo construtor

                        arguments.Add(service);
                    }
                }

                // Se conseguimos preencher todos os parâmetros, criamos a instância
                if (arguments.Count == parameters.Length)
                {
                    return Activator.CreateInstance(type, arguments.ToArray());
                }
            }

            throw new InvalidOperationException($"Não foi possível resolver a instância de {type.Name}.");
        }
    }
}
