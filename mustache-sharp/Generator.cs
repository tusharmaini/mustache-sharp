﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Mustache
{
    /// <summary>
    /// Generates text by substituting an object's values for placeholders.
    /// </summary>
    public sealed class Generator
    {
        private readonly IGenerator _generator;
        private readonly List<EventHandler<KeyFoundEventArgs>> _foundHandlers;
        private readonly List<EventHandler<KeyNotFoundEventArgs>> _notFoundHandlers;

        /// <summary>
        /// Initializes a new instance of a Generator.
        /// </summary>
        /// <param name="generator">The text generator to wrap.</param>
        internal Generator(IGenerator generator)
        {
            _generator = generator;
            _foundHandlers = new List<EventHandler<KeyFoundEventArgs>>();
            _notFoundHandlers = new List<EventHandler<KeyNotFoundEventArgs>>();
        }

        /// <summary>
        /// Occurs when a key/property is found.
        /// </summary>
        public event EventHandler<KeyFoundEventArgs> KeyFound
        {
            add { _foundHandlers.Add(value); }
            remove { _foundHandlers.Remove(value); }
        }

        /// <summary>
        /// Occurs when a key/property is not found in the object graph.
        /// </summary>
        public event EventHandler<KeyNotFoundEventArgs> KeyNotFound
        {
            add { _notFoundHandlers.Add(value); }
            remove { _notFoundHandlers.Remove(value); }
        }

        /// <summary>
        /// Gets the text that is generated for the given object.
        /// </summary>
        /// <param name="source">The object to generate the text with.</param>
        /// <returns>The text generated for the given object.</returns>
        public string Render(object source)
        {
            return render(CultureInfo.CurrentCulture, source);
        }

        /// <summary>
        /// Gets the text that is generated for the given object.
        /// </summary>
        /// <param name="provider">The format provider to use.</param>
        /// <param name="source">The object to generate the text with.</param>
        /// <returns>The text generated for the given object.</returns>
        public string Render(IFormatProvider provider, object source)
        {
            if (provider == null)
            {
                provider = CultureInfo.CurrentCulture;
            }
            return render(provider, source);
        }

        private string render(IFormatProvider provider, object source)
        {
            KeyScope scope = new KeyScope(source);
            foreach (EventHandler<KeyFoundEventArgs> handler in _foundHandlers)
            {
                scope.KeyFound += handler;
            }
            foreach (EventHandler<KeyNotFoundEventArgs> handler in _notFoundHandlers)
            {
                scope.KeyNotFound += handler;
            }
            StringWriter writer = new StringWriter(provider);
            _generator.GetText(scope, writer, null);
            return writer.ToString();
        }
    }
}
