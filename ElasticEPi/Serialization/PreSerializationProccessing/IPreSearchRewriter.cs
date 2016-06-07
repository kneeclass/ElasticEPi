using System;

namespace ElasticEPi.Serialization.PreSerializationProccessing {
    interface IPreSearchModifier {

        void ModifySearch(object data);

        Type ModifysType { get; }

    }
}
