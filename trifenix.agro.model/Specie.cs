﻿using Cosmonaut;
using Cosmonaut.Attributes;

namespace trifenix.agro.db.model.agro {

    [SharedCosmosCollection("agro", "Specie")]
    public class Specie : DocumentBaseName, ISharedCosmosEntity {
        public override string Id { get; set; }

        public override string Name { get; set; }

        public string Abbreviation { get; set; }
    }

}
