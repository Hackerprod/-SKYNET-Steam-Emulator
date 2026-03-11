using System;
using System.Collections.Generic;

namespace SKYNET.Steamworks
{
    public static class ContextManager
    {
        private static readonly Dictionary<IntPtr, Context> _contexts = new Dictionary<IntPtr, Context>();
        private static IntPtr _nextContextId = (IntPtr)1; // Comienza desde 1 (0 se usa para errores).

        /// <summary>
        /// Inicializa un nuevo contexto o devuelve uno existente si ya está inicializado.
        /// </summary>
        /// <param name="pContextInitData">Datos opcionales para inicialización.</param>
        /// <returns>Un puntero único al contexto inicializado.</returns>
        public static IntPtr InitializeContext(IntPtr pContextInitData)
        {
            lock (_contexts)
            {
                // Validar datos de inicialización
                if (pContextInitData == IntPtr.Zero)
                {
                    Console.WriteLine("Error: Datos de inicialización no válidos.");
                    return IntPtr.Zero;
                }

                // Verificar si el contexto ya existe
                foreach (var context in _contexts.Values)
                {
                    if (context.InitData == pContextInitData)
                    {
                        Console.WriteLine($"Contexto ya existente con ID: {context.Id}");
                        return context.Id;
                    }
                }

                // Crear un nuevo contexto
                var newContext = new Context
                {
                    Id = _nextContextId,
                    InitData = pContextInitData
                };

                _contexts[newContext.Id] = newContext;
                Console.WriteLine($"Nuevo contexto inicializado con ID: {_nextContextId}");

                _nextContextId = (IntPtr)((long)_nextContextId + 1); // Incrementar ID para el próximo contexto.
                return newContext.Id;
            }
        }
    }
    /// <summary>
    /// Representa un contexto de SteamWorks.
    /// </summary>
        public class Context
    {
        public IntPtr Id { get; set; }
        public IntPtr InitData { get; set; }
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
    }
}
