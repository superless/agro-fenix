﻿using Microsoft.Azure.Search;
using System;
using System.ComponentModel.DataAnnotations;

namespace trifenix.agro.search.model {

    public class EntitySearch {

        [Key]
        [IsFilterable]
        public string Id { get; set; }                              

        [IsFilterable]
        public int EntityIndex { get; set; }                        

        [IsSortable]
        public DateTime Created { get; set; }                       

        public RelatedId[] RelatedIds { get; set; } 
        
        public Property[] RelatedProperties { get; set; }       

        public RelatedEnumValue[] RelatedEnumValues { get; set; }

    }

    public class RelatedId {

        [IsFilterable]
        public int EntityIndex { get; set; }

        [IsFilterable, IsSearchable]
        public string EntityId { get; set; }

        


    }

    public class Property {

        [IsFilterable]
        public int PropertyIndex { get; set; }

        [IsSearchable, IsFilterable]
        public string Value { get; set; }



    }

    public class RelatedEnumValue {

        [IsFilterable]
        public int EnumerationIndex { get; set; }

        [IsFilterable]
        public int Value { get; set; }

    }

}