using Caliburn.Micro;

namespace Readit.WPF.Infrastructure
{
    public static class DependencyResolver
    {
        private static SimpleContainer _container;

        /// <summary>
        /// Configura o contêiner
        /// </summary>
        public static void SetContainer(SimpleContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolve um serviço pelo tipo
        /// </summary>
        public static T GetService<T>()
        {
            if (_container == null)
                throw new InvalidOperationException("Contêiner não configurado.");

            return (T)_container.GetInstance(typeof(T), null);
        }

        /// <summary>
        /// Criar uma instância resolvendo dependências e permitindo parâmetros dinâmicos
        /// </summary>
        public static object CreateInstance(Type type, params object[] extraArgs)
        {
            if (_container == null)
                throw new InvalidOperationException("Contêiner não configurado.");

            var constructors = type.GetConstructors()
                           .OrderByDescending(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var arguments = new List<object>();
                var usedArgs = new HashSet<int>(); // Guarda os índices dos argumentos já usados

                foreach (var param in parameters)
                {
                    // Procura um argumento extra que ainda não tenha sido usado
                    var argumentIndex = Array.FindIndex(extraArgs, a =>
                        a != null && param.ParameterType.IsAssignableFrom(a.GetType()) && !usedArgs.Contains(Array.IndexOf(extraArgs, a)));

                    if (argumentIndex != -1)
                    {
                        arguments.Add(extraArgs[argumentIndex]);
                        usedArgs.Add(argumentIndex); // Marca esse argumento como usado
                    }
                    else
                    {
                        // Se não encontrar um argumento, tenta resolver via DI
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