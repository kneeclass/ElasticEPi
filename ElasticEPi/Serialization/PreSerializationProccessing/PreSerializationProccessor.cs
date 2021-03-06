﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ElasticEPi.Extensions;
using ElasticEPi.Logging;
using Elasticsearch.Net.Serialization;
using EPiServer.Logging;
using Nest;
using Newtonsoft.Json;

namespace ElasticEPi.Serialization.PreSerializationProccessing {
    public class PreSerializationProccessor : NestSerializer {
        public PreSerializationProccessor(IConnectionSettingsValues settings) : base(settings) {}

        public override byte[] Serialize(object data, SerializationFormatting formatting = SerializationFormatting.Indented) {

            var modifier = GetModifier(data);
            if (modifier != null)
                modifier.ModifySearch(data);
            try {
                return base.Serialize(data, formatting);
            }
            catch (JsonWriterException exception) {
                Logger.WriteToLog($"PreSerializationProccessor failed. data type: {data.GetType().Name}",Level.Error,exception);
                return null;
            }
        }


        private IPreSearchModifier GetModifier(object data) {
            var key = Modifiers.Keys.SingleOrDefault(x => data.GetType().IsGenericTypeOf(x));
            return key == null ? null : Modifiers[key];
        }

        private Dictionary<Type, IPreSearchModifier> _modifiers;
        private Dictionary<Type, IPreSearchModifier> Modifiers {
            get {
                if (_modifiers == null) {
                    var preSearchModifier = typeof (IPreSearchModifier);
                    var preSearchModifiers = from type in Assembly.GetAssembly(GetType()).GetTypes()
                        where !type.IsInterface && preSearchModifier.IsAssignableFrom(type)
                        select type;

                    _modifiers = preSearchModifiers.Select(Activator.CreateInstance).ToDictionary(x => ((IPreSearchModifier)x).ModifysType, y => (IPreSearchModifier)y);

                }
                return _modifiers;
            }
        }

        
    }
}
