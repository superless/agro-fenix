﻿using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using trifenix.agro.attr;
using trifenix.agro.db;
using trifenix.agro.enums;
using trifenix.agro.enums.query;
using trifenix.agro.enums.search;
using trifenix.agro.enums.searchModel;
using trifenix.agro.search.interfaces;
using trifenix.agro.search.model;
using V2 = trifenix.agro.search.model.temp;

namespace trifenix.agro.search.operations {
    
    public class AgroSearch : IAgroSearch {
    
        private readonly SearchServiceClient _search;
        private readonly string _entityIndex = "entities";
        private readonly string _commentIndex = "comments";

        private readonly ISearchQueries _queries;

        // para activar auto-complete, no es activado aún debido a el tamaño que toma.
        //private readonly List<Suggester> _suggesterEntities = new List<Suggester> {
        //    new Suggester {
        //        Name= "SgProperty",
        //        SourceFields = new List<string>{ 
        //            "RelatedProperties/Value"
        //        }
        //    }
        //};

        public AgroSearch(string SearchServiceName, string SearchServiceKey) {
            _queries = new SearchQueries();
            _search = new SearchServiceClient(SearchServiceName, new SearchCredentials(SearchServiceKey));
            if (!_search.Indexes.Exists(_entityIndex))
                _search.Indexes.CreateOrUpdate(new Index { Name = _entityIndex, Fields = FieldBuilder.BuildForType<EntitySearch>() });
            if (!_search.Indexes.Exists(_commentIndex))
                _search.Indexes.CreateOrUpdate(new Index { Name = _commentIndex, Fields = FieldBuilder.BuildForType<CommentSearch>() });
        }

        private string Queries(SearchQuery query) => _queries.Get(query);

        private void OperationElements<T>(List<T> elements, SearchOperation operationType) {
            var indexName = typeof(T).Equals(typeof(EntitySearch)) ? _entityIndex : _commentIndex;
            var indexClient = _search.Indexes.GetClient(indexName);
            var actions = elements.Select(o => operationType == SearchOperation.Add ? IndexAction.Upload(o) : IndexAction.Delete(o));
            var batch = IndexBatch.New(actions);
            indexClient.Documents.Index(batch);
        }

        public void AddElements<T>(List<T> elements) {
            OperationElements(elements, SearchOperation.Add);
        }

        public void DeleteElements<T>(List<T> elements) {
            OperationElements(elements, SearchOperation.Delete);
        }

        public List<T> FilterElements<T>(string filter) {
            var indexName = typeof(T).Equals(typeof(EntitySearch)) ? _entityIndex : _commentIndex;
            var indexClient = _search.Indexes.GetClient(indexName);
            var result = indexClient.Documents.Search<T>(null, new SearchParameters { Filter = filter });
            return result.Results.Select(v => v.Document).ToList();
        }

        public void DeleteElements<T>(string query) {
            var elements = FilterElements<EntitySearch>(query);
            if (elements.Any())
                DeleteElements(elements);
        }

        public EntitySearch GetEntity(EntityRelated entityRelated, string id) {
            var indexClient = _search.Indexes.GetClient(_entityIndex);
            var entity = indexClient.Documents.Search<EntitySearch>(null, new SearchParameters { Filter = string.Format(Queries(SearchQuery.GET_ELEMENT), (int)entityRelated, id) }).Results.FirstOrDefault()?.Document;
            return entity;
        }

        public void DeleteElementsWithRelatedElement(EntityRelated elementToDelete, EntityRelated relatedElement, string idRelatedElement) {
            var query = string.Format(Queries(SearchQuery.ENTITIES_WITH_ENTITYID), (int)elementToDelete, (int)relatedElement, idRelatedElement);
            DeleteElements<EntitySearch>(query);
        }

        public void DeleteElementsWithRelatedElementExceptId(EntityRelated elementToDelete, EntityRelated relatedElement, string idRelatedElement, string elementExceptId) {
            var query = string.Format(Queries(SearchQuery.ENTITIES_WITH_ENTITYID_EXCEPTID), (int)elementToDelete, (int)relatedElement, idRelatedElement, elementExceptId);
            DeleteElements<EntitySearch>(query);
        }

        public void DeleteEntity(EntityRelated entityRelated, string id) {
            var query = string.Format(Queries(SearchQuery.GET_ELEMENT), (int)entityRelated, id);
            DeleteElements<EntitySearch>(query);
        }


        private V2.BaseProperty<T> GetProperty<T>(int index, object value) {
            var element = (V2.BaseProperty<T>)Activator.CreateInstance(typeof(V2.BaseProperty<T>));
            element.PropertyIndex = index;
            element.Value = (T)value;
            return element;
        }

        public IEnumerable<V2.RelatedId> GetArrayOfRelatedIds(KeyValuePair<SearchAttribute, object> attribute) {

            var typeValue = attribute.Value.GetType();
            if (typeValue == typeof(IEnumerable<string>))
            {
                return ((IEnumerable<string>)attribute.Value).Select(s => new V2.RelatedId { EntityIndex = attribute.Key.Index, EntityId=(string)s });
               
            }
            else
            {
                return new List<V2.RelatedId>() { new V2.RelatedId { EntityIndex = attribute.Key.Index, EntityId = (string)attribute.Value } };

            }

        }

        private IEnumerable<V2.BaseProperty<T>> GetArrayOfElements<T>(KeyValuePair<SearchAttribute, object> attribute)
        {
            var typeValue = attribute.Value.GetType();
            if (typeValue == typeof(IEnumerable<T>))
            {
                return ((IEnumerable<T>)attribute.Value).Select(s => GetProperty<T>(attribute.Key.Index, s));
            }
            else {
                return new List<V2.BaseProperty<T>> { GetProperty<T>(attribute.Key.Index, attribute.Value)  };

            }
        }

        private IEnumerable<V2.BaseProperty<T>> GetPropertiesObjects<T>(Related related, Dictionary<SearchAttribute, object> elements) { 
            return elements.Where(s => s.Key.Related == related).SelectMany(s => GetArrayOfElements<T>(s)).ToArray();
        }

        private V2.RelatedId[] GetReferences(Dictionary<SearchAttribute, object> elements)
        {
            return elements.Where(s => s.Key.Related == Related.REFERENCE).SelectMany(GetArrayOfRelatedIds).ToArray();

        }

        private V2.RelatedId[] GetLocalReferences(Dictionary<SearchAttribute, object> elements)
        {
            return elements.Where(s => s.Key.Related == Related.LOCAL_REFERENCE).SelectMany(GetArrayOfRelatedIds).ToArray();

        }



        

        private V2.EntitySearch[] GetEntitySearch(object obj, int index, string id) {

            var list = new List<V2.EntitySearch>();

            var entitySearch = new V2.EntitySearch() { };
            entitySearch.Id = id;
            entitySearch.EntityIndex = index;
            entitySearch.Created = DateTime.Now;

            var values = GetPropertiesByAttribute<SearchAttribute>(obj);

            if (!values.Any()) return Array.Empty<V2.EntitySearch>();

            entitySearch.NumProperties = (V2.Num32Property[])GetPropertiesObjects<int>(Related.NUM32, values);

            entitySearch.DoubleProperties = (V2.DblProperty[])GetPropertiesObjects<double>(Related.DBL, values);

            entitySearch.DtProperties = (V2.DtProperty[])GetPropertiesObjects<DateTime>(Related.DATE, values);

            entitySearch.EnumProperties = (V2.EnumProperty[])GetPropertiesObjects<int>(Related.ENUM, values);

            entitySearch.GeoProperties = (V2.GeoProperty[])GetPropertiesObjects<GeographyPoint>(Related.GEO, values);

            entitySearch.Num64Properties = (V2.Num64Property[])GetPropertiesObjects<long>(Related.NUM64, values);

            entitySearch.StringProperties = (V2.StrProperty[])GetPropertiesObjects<string>(Related.STR, values);


            entitySearch.SuggestProperties = (V2.SuggestProperty[])GetPropertiesObjects<string>(Related.SUGGESTION, values);

            entitySearch.RelatedIds = GetReferences(values);


            var valuesWithoutProperty = GetPropertiesWithoutAttribute<SearchAttribute>(obj);

            foreach (var item in valuesWithoutProperty)
            {
                var value = GetEntitySearch(item, 0, string.Empty);

                entitySearch.NumProperties = entitySearch.NumProperties.Union(value.SelectMany(s=>s.NumProperties)).ToArray();

                entitySearch.DoubleProperties = entitySearch.DoubleProperties.Union(value.SelectMany(s => s.DoubleProperties)).ToArray();

                entitySearch.DtProperties = entitySearch.DtProperties.Union(value.SelectMany(s => s.DtProperties)).ToArray();

                entitySearch.EnumProperties = entitySearch.EnumProperties.Union(value.SelectMany(s => s.EnumProperties)).ToArray();

                entitySearch.GeoProperties = entitySearch.GeoProperties.Union(value.SelectMany(s => s.GeoProperties)).ToArray();

                entitySearch.Num64Properties = entitySearch.Num64Properties.Union(value.SelectMany(s => s.Num64Properties)).ToArray();

                entitySearch.StringProperties = entitySearch.StringProperties.Union(value.SelectMany(s => s.StringProperties)).ToArray();


                entitySearch.SuggestProperties = entitySearch.SuggestProperties.Union(value.SelectMany(s => s.SuggestProperties)).ToArray();

                entitySearch.RelatedIds = entitySearch.RelatedIds.Union(value.SelectMany(s => s.RelatedIds)).ToArray();

            }
            var localReference = GetLocalReferences(values);

            if (localReference.Any())
            {
                foreach (var item in localReference)
                {
                    var guid = Guid.NewGuid().ToString("N");
                    var localEntities = GetEntitySearch(item, item.EntityIndex, guid);
                    var listReferences = entitySearch.RelatedIds.ToList();
                    listReferences.Add(new V2.RelatedId { EntityId = guid, EntityIndex = item.EntityIndex });
                    entitySearch.RelatedIds = listReferences.ToArray();
                    list.AddRange(localEntities);
                    
                }
            }

            list.Add(entitySearch);

            return list.ToArray();




        }

        public V2.EntitySearch[] GetEntitySearch<T>(T entity)  where T : DocumentBase {

            var reference = typeof(T).GetTypeInfo().GetCustomAttribute<ReferenceSearchAttribute>(true);
            if (reference == null) return Array.Empty<V2.EntitySearch>();
            return GetEntitySearch(entity, reference.Index, entity.Id);
        }

        private Dictionary<SearchAttribute, object> GetPropertiesByAttribute<T_Attr>(object Obj) where T_Attr : SearchAttribute => Obj.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(T_Attr), true) && prop.GetValue(Obj) != null).ToDictionary(prop => (SearchAttribute)prop.GetCustomAttributes(typeof(T_Attr), true).FirstOrDefault(), prop => prop.GetValue(Obj));



        private object[] GetPropertiesWithoutAttribute<T_Attr>(object Obj) where T_Attr : SearchAttribute => 
            Obj.GetType().GetProperties().Where(prop => !Attribute.IsDefined(prop, typeof(T_Attr), true) && prop.GetValue(Obj) != null).
            Select(prop => prop.GetValue(Obj)).ToArray();

    }

}