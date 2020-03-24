﻿using Cosmonaut;
using Cosmonaut.Attributes;
using trifenix.agro.attr;
using trifenix.agro.enums;

namespace trifenix.agro.db.model.agro {

    [SharedCosmosCollection("agro", "Specie")]
    public class Specie : DocumentBaseName, ISharedCosmosEntity {
        public override string Id { get; set; }


        [StringSearch(StringRelated.GENERIC_NAME)]
        public override string Name { get; set; }

        [StringSearch(StringRelated.GENERIC_ABBREVIATION)]
        public string Abbreviation { get; set; }
    }

}
