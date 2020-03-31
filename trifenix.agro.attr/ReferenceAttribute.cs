﻿using System;

using trifenix.agro.enums.searchModel;
using trifenix.agro.search.model.reflection;

namespace trifenix.agro.attr {

    public class SearchAttribute : Attribute
    {

        public virtual Related Related { get; }
        public virtual int Index { get; }
    }




    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class ReferenceSearchAttribute : SearchAttribute
    {
        private readonly EntityRelated _index;
        public bool Local { get; }
        public ReferenceSearchAttribute(EntityRelated index, bool local = false) {
            _index = index;
            Local = local;
        }

        
        public override int Index => (int)_index;
        public override Related Related => Local ? Related.LOCAL_REFERENCE : Related.REFERENCE;
    }

    public class GroupAttribute {
        public GroupAttribute(int index, string title = "", Device device = Device.Web)
        {
            Group = new GroupInput
            {
                Index = index,
                Name = title,
                Device = device
            };
        }

        public GroupInput Group { get; }
    }

    public class StringSearchAttribute : SearchAttribute {
        private readonly StringRelated _index;
        public StringSearchAttribute(StringRelated index) {
            _index = index;
        }
        public override int Index => (int)_index;
        public override Related Related => Related.STR;
    }

    public class SuggestSearchAttribute : SearchAttribute {
        private readonly StringRelated _index;
        public SuggestSearchAttribute(StringRelated index) {
            _index = index;
        }
        public override int Index => (int)_index;
        public override Related Related => Related.SUGGESTION;
    }

    public class Num32SearchAttribute : SearchAttribute {
        private readonly NumRelated _index;
        public Num32SearchAttribute(NumRelated index) {
            _index = index;
        }
        public override int Index => (int)_index;
        public override Related Related => Related.NUM32;
    }

    public class Num64SearchAttribute : SearchAttribute {
        private readonly NumRelated _index;
        public Num64SearchAttribute(NumRelated index) {
            _index = index;
        }
        public override int Index => (int)_index;
        public override Related Related => Related.NUM64;
    }

    public class DoubleSearchAttribute : SearchAttribute {
        private readonly DoubleRelated _index;
        public DoubleSearchAttribute(DoubleRelated index) {
            _index = index;
        }
        public override int Index => (int)_index;
        public override Related Related => Related.DBL;
    }

    public class BoolSearchAttribute : SearchAttribute {
        private readonly BoolRelated _index;
        public BoolSearchAttribute(BoolRelated index) {
            _index = index;
        }
        public override int Index => (int)_index;
        public override Related Related => Related.BOOL;
    }

    public class GeoSearchAttribute : SearchAttribute {
        private readonly GeoRelated _index;
        public GeoSearchAttribute(GeoRelated index) {
            _index = index;
        }
        public override int Index => (int)_index;
        public override Related Related => Related.GEO;
    }

    public class EnumSearchAttribute : SearchAttribute {
        private readonly EnumRelated _index;
        public EnumSearchAttribute(EnumRelated index) {
            _index = index;
        }
        public override int Index => (int)_index;
        public override Related Related => Related.ENUM;


    }

    public class DateSearchAttribute : SearchAttribute {
        private readonly DateRelated _index;
        public DateSearchAttribute(DateRelated index) {
            _index = index;
        }
        public override int Index => (int)_index;
        public override Related Related => Related.DATE;
    }

    

}