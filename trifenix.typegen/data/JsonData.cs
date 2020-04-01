﻿using res.core;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using trifenix.agro.attr;
using trifenix.agro.db.model;
using trifenix.agro.enums.searchModel;
using trifenix.agro.search.model.reflection;
using trifenix.agro.search.model.ts;
using TypeGen.Core.Extensions;
using static trifenix.agro.search.operations.util.AgroHelper;

namespace trifenix.typegen.data
{
    public static class JsonData
    {
        public static ModelDictionary GetModel(IEnumerable<PropertySearchInfo> propertySearchInfos, EntityRelated index)
        {
            var propByRelatedAndIndex = propertySearchInfos.GroupBy(s => new {  s.Related, s.IndexClass, s.Index }).Select(s => s.FirstOrDefault());

            

            var boolEnums = GetDescription(typeof(BoolRelated));

            var stringEnum = GetDescription(typeof(StringRelated));

            var doubleEnum = GetDescription(typeof(DoubleRelated));

            var dateEnum = GetDescription(typeof(DateRelated));

            var geoEnum = GetDescription(typeof(GeoRelated));

            var numEnum = GetDescription(typeof(NumRelated));

            var enumEmun = GetDescription(typeof(EnumRelated));

            var enumRelated = GetDescription(typeof(EntityRelated));


            var modelInfo = ResourceExtension.ResourceModel(Related.REFERENCE, propByRelatedAndIndex.FirstOrDefault().IndexClass);

            var modelDictionary = new ModelDictionary()
            {
                Index = index,

                Description = modelInfo.Description,

                ShortName = modelInfo.ShortName,

                Title = modelInfo.Title,

                BoolData = GetDictionaryFromRelated(propByRelatedAndIndex, Related.BOOL),

                StringData = GetDictionaryFromRelated(propByRelatedAndIndex, Related.STR),

                DateData = GetDictionaryFromRelated(propByRelatedAndIndex, Related.DATE),

                DoubleData = GetDictionaryFromRelated(propByRelatedAndIndex, Related.DBL),

                EnumData = GetEnumDictionaryFromRelated(propByRelatedAndIndex, enumEmun),

                GeoData = GetDictionaryFromRelated(propByRelatedAndIndex, Related.GEO),

                NumData = GetDictionaryFromRelated(propByRelatedAndIndex, Related.NUM32),

                



            };

            //include suggestion, Refererence Local and num64,
            var suggestions = GetDictionaryFromRelated(propByRelatedAndIndex, Related.SUGGESTION);

            var referenceLocal = GetDictionaryFromRelated(propByRelatedAndIndex, Related.LOCAL_REFERENCE);

            var num64 = GetDictionaryFromRelated(propByRelatedAndIndex, Related.NUM64);

            var suggestionNotInString = suggestions.Where(sg => !modelDictionary.StringData.Any(s => s.Key == sg.Key));

            

            var num64NotInNum = num64.Where(sg => !modelDictionary.NumData.Any(s => s.Key == sg.Key));

            if (suggestionNotInString.Any())
            {
                foreach (var item in suggestionNotInString)
                {
                    modelDictionary.StringData.Add(item.Key, item.Value);
                }
            }

            

            if (num64NotInNum.Any())
            {
                foreach (var item in num64NotInNum)
                {
                    modelDictionary.NumData.Add(item.Key, item.Value);
                }
            }




            return modelDictionary;

        }
        public static ModelMetaData GetJsonData() {

            // get assemblu
            var assembly = Assembly.GetAssembly(typeof(Barrack));

            //get model types from namespace
            var modelTypes = assembly.GetLoadableTypes()
            .Where(x => x.FullName.StartsWith("trifenix.agro.db.model"));

            // get property infos
            var propSearchinfos = modelTypes.Where(s => s.GetTypeInfo().GetCustomAttributes<ReferenceSearchAttribute>(true).Any()).SelectMany(GetPropertySearchInfo).ToList();

            var grpIndexes = propSearchinfos.GroupBy(s => s.IndexClass).Select(s=>s.FirstOrDefault()) ;


            var model = grpIndexes.Select(g=> GetModel(propSearchinfos.Where(s => s.IndexClass == g.IndexClass), (EntityRelated)g.IndexClass));


            return new ModelMetaData {
                Indexes = model.ToArray()
            };
            
            // group by index and related, get the first element
            

        }



 
        private static Dictionary<int, DefaultDictionary> GetDictionaryFromRelated(IEnumerable<PropertySearchInfo> propSearchInfos, Related related) {

            var infos = propSearchInfos.Where(s => s.Related == related).ToList();

            return infos.ToDictionary(s => s.Index, g => new DefaultDictionary
            {
                NameProp = g.Name,
                isArray = g.IsEnumerable,
                Info = g.Info
            });
        }

        private static Dictionary<int, EnumDictionary> GetEnumDictionaryFromRelated(IEnumerable<PropertySearchInfo> propSearchInfos, Dictionary<int, string> enumDescription)
        {

            return propSearchInfos.Where(s => s.Related == Related.ENUM).ToDictionary(s => s.Index, g => new EnumDictionary
            {
                NameProp = g.Name,
                isArray = g.IsEnumerable,
                Info = g.Info,
                EnumData = g.Enums
            });
        }




    }
}
