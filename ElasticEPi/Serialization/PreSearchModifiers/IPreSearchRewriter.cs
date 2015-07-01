using System;

namespace ElasticEPi.Serialization.PreSearchModifiers {
    interface IPreSearchModifier {

        void ModifySearch(object data);

        Type ModifysType { get; }

    }
}
