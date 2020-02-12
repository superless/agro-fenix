using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using trifenix.agro.db.model.agro.enums;
using trifenix.agro.db.model.agro.orders;
using trifenix.agro.db.model.agro;
using trifenix.agro.external.operations.helper;
using trifenix.agro.functions.Helper;
using trifenix.agro.model.external.Input;
using trifenix.agro.model.external.output;
using trifenix.agro.model.external;
using trifenix.agro.search.model;
using trifenix.agro.email.operations;
using trifenix.agro.db.model.agro.core;

namespace trifenix.agro.functions {
    public static class MainAgroFunction {

        #region v2/phenological_events
        [FunctionName("PhenologicalEventV2")]
        public static async Task<IActionResult> PhenologicalEventV2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/phenological_events/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<List<PhenologicalEvent>> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    result = await manager.PhenologicalEvents.GetPhenologicalEvents();
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var initDate = (DateTime)model["startDate"];
                        var endDate = (DateTime)model["endDate"];
                        return await db.PhenologicalEvents.SaveNewPhenologicalEvent(name, initDate, endDate);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var initDate = (DateTime)model["startDate"];
                        var endDate = (DateTime)model["endDate"];
                        return await db.PhenologicalEvents.SaveEditPhenologicalEvent(id, name, initDate, endDate);
                    }, claims);
            }
            return ContainerMethods.GetJsonGetContainer(result, log);
        }
        #endregion

        #region v2/seasons
        [FunctionName("SeasonV2")]
        public static async Task<IActionResult> SeasonV2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/seasons/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<List<Season>> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    result = await manager.Seasons.GetSeasons();
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var initDate = (DateTime)model["startDate"];
                        var endDate = (DateTime)model["endDate"];
                        return await db.Seasons.SaveNewSeason(initDate, endDate);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var initDate = (DateTime)model["startDate"];
                        var endDate = (DateTime)model["endDate"];
                        var current = (bool)model["current"];
                        return await db.Seasons.SaveEditSeason(id, initDate, endDate, current);
                    }, claims);
            }
            return ContainerMethods.GetJsonGetContainer(result, log);
        }
        #endregion

        #region v2/specie
        [FunctionName("SpecieV2")]
        public static async Task<IActionResult> SpecieV2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/species/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<Specie> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id)) {
                        result = await manager.Species.GetSpecie(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var abbreviation = (string)model["abbreviation"];
                        return await db.Species.SaveNewSpecie(name, abbreviation);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var abbreviation = (string)model["abbreviation"];
                        return await db.Species.SaveEditSpecie(id, name, abbreviation);
                    }, claims);
            }
            ExtGetContainer<List<Specie>> resultGetAll = await manager.Species.GetSpecies();
            return ContainerMethods.GetJsonGetContainer(resultGetAll, log);
        }
        #endregion

        #region v2/rootstock
        [FunctionName("RootstockV2")]
        public static async Task<IActionResult> RootstockV2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/rootstock/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<List<Rootstock>> result = null;
            switch (req.Method.ToLower())
            {
                case "get":
                    result = await manager.Rootstock.GetRootstocks();
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var abbreviation = (string)model["abbreviation"];
                        return await db.Rootstock.SaveNewRootstock(name, abbreviation);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var abbreviation = (string)model["abbreviation"];
                        return await db.Rootstock.SaveEditRootstock(id, name, abbreviation);
                    }, claims);
            }
            return ContainerMethods.GetJsonGetContainer(result, log);
        }
        #endregion

        #region v2/certified_entities
        [FunctionName("CertifiedEntity")]
        public static async Task<IActionResult> CertifiedEntity([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/certified_entities/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<CertifiedEntity> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        result = await manager.CertifiedEntities.GetCertifiedEntity(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var abbreviation = (string)model["abbreviation"];
                        return await db.CertifiedEntities.SaveNewCertifiedEntity(name, abbreviation);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var abbreviation = (string)model["abbreviation"];
                        return await db.CertifiedEntities.SaveEditCertifiedEntity(id, name, abbreviation);
                    }, claims);
            }
            ExtGetContainer<List<CertifiedEntity>> resultGetAll = await manager.CertifiedEntities.GetCertifiedEntities();
            return ContainerMethods.GetJsonGetContainer(resultGetAll, log);
        }
        #endregion

        #region v2/ingredient_categories
        [FunctionName("CategoryIngredientsV2")]
        public static async Task<IActionResult> CategoryIngredientsV2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/ingredient_categories/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<List<IngredientCategory>> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    result = await manager.IngredientCategories.GetIngredientCategories();
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        return await db.IngredientCategories.SaveNewIngredientCategory(name);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        return await db.IngredientCategories.SaveEditIngredientCategory(id, name);
                    }, claims);
            }
            return ContainerMethods.GetJsonGetContainer(result, log);
        }
        #endregion

        #region v2/ingredients
        [FunctionName("IngredientsV2")]
        public static async Task<IActionResult> IngredientsV2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/ingredients/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<List<Ingredient>> result = null;
            switch (req.Method.ToLower())
            {
                case "get":
                    result = await manager.Ingredients.GetIngredients();
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var idCategory = (string)model["idCategory"];
                        return await db.Ingredients.SaveNewIngredient(name, idCategory);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var idCategory = (string)model["idCategory"];
                        return await db.Ingredients.SaveEditIngredient(id, name, idCategory);
                    }, claims);
            }
            return ContainerMethods.GetJsonGetContainer(result, log);
        }
        #endregion

        #region v2/targets
        [FunctionName("TargetV2")]
        public static async Task<IActionResult> TargetV2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/targets/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<List<ApplicationTarget>> result = null;
            switch (req.Method.ToLower())
            {
                case "get":
                    result = await manager.ApplicationTargets.GetAplicationsTarget();
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        return await db.ApplicationTargets.SaveNewApplicationTarget(name);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        return await db.ApplicationTargets.SaveEditApplicationTarget(id, name);
                    }, claims);
            }
            return ContainerMethods.GetJsonGetContainer(result, log);
        }
        #endregion

        #region v2/order_folders
        [FunctionName("OrderFolder")]
        public static async Task<IActionResult> OrderFolder([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/order_folders/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<OrderFolder> result = null;
            switch (req.Method.ToLower())
            {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        result = await manager.OrderFolder.GetOrderFolder(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var idPhenologicalEvent = (string)model["idPhenologicalEvent"];
                        var idApplicationTarget = (string)model["idApplicationTarget"];
                        var categoryId = (string)model["idCategory"];
                        var idSpecie = (string)model["idSpecie"];
                        var idIngredient = (string)model["idIngredient"];
                        return await db.OrderFolder.SaveNewOrderFolder(idPhenologicalEvent, idApplicationTarget, categoryId, idSpecie, idIngredient);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var idPhenologicalEvent = (string)model["idPhenologicalEvent"];
                        var idApplicationTarget = (string)model["idApplicationTarget"];
                        var idCategory = (string)model["idCategory"];
                        var idSpecie = (string)model["idSpecie"];
                        var idIngredient = (string)model["idIngredient"];
                        return await db.OrderFolder.SaveEditOrderFolder(id, idPhenologicalEvent, idApplicationTarget, idCategory, idSpecie, idIngredient);
                    }, claims);
            }
            ExtGetContainer<List<OrderFolder>> resultGetAll = await manager.OrderFolder.GetOrderFolders();
            return ContainerMethods.GetJsonGetContainer(resultGetAll, log);
        }
        #endregion

        #region v2/products
        [FunctionName("Product")]
        public static async Task<IActionResult> Product([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/products/{idProduct?}/{textToSearch?}/{asc?}/{totalByPage?}/{page?}")] HttpRequest req, string idProduct, string textToSearch, string asc, int? totalByPage, int? page, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<Product> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(idProduct) && !idProduct.ToLower().Equals("all")) {
                        result = await manager.Products.GetProduct(idProduct);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations<string>(req.Body, log, async (db, model) => {
                        var commercialName = (string)model["commercialName"];
                        var idActiveIngredient = (string)model["idActiveIngredient"];
                        var brand = (string)model["brand"];
                        var measureType = (MeasureType)Convert.ToInt32(model["measureType"]);
                        var quantity = (int)model["quantity"];
                        var kindOfBottle = (KindOfProductContainer)Convert.ToInt32(model["kindOfBottle"]);
                        var dosesStr = ((object)model["doses"])?.ToString();
                        var doses = !string.IsNullOrWhiteSpace(dosesStr) ? JsonConvert.DeserializeObject<DosesInput[]>(dosesStr) : null;
                        return await db.Products.CreateProduct(commercialName, idActiveIngredient, brand, doses, measureType, quantity, kindOfBottle);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations<Product>(req.Body, log, async (db, model) => {
                        var commercialName = (string)model["commercialName"];
                        var idActiveIngredient = (string)model["idActiveIngredient"];
                        var brand = (string)model["brand"];
                        var measureType = (MeasureType)Convert.ToInt32(model["measureType"]);
                        var quantity = (int)model["quantity"];
                        var kindOfBottle = (KindOfProductContainer)Convert.ToInt32(model["kindOfBottle"]);
                        var dosesStr = ((object)model["doses"])?.ToString();
                        var doses = !string.IsNullOrWhiteSpace(dosesStr) ? JsonConvert.DeserializeObject<DosesInput[]>(dosesStr) : null;
                        return await db.Products.SaveEditProduct(idProduct, commercialName, idActiveIngredient, brand, doses, measureType, quantity, kindOfBottle);
                    }, claims);
            }
            if (!string.IsNullOrWhiteSpace(textToSearch) && textToSearch.Equals("*"))
                textToSearch = null;
            bool? order = null;
            if (!string.IsNullOrWhiteSpace(asc) && !asc.ToLower().Equals("not_order")) {
                if (asc.ToLower().Equals("asc")) order = true;
                else if (asc.ToLower().Equals("desc")) order = false;
                else return new BadRequestResult();
            }
            var resultGetByPageAll = manager.Products.GetPaginatedProducts(textToSearch, page, totalByPage, order);
            return ContainerMethods.GetJsonGetContainer(resultGetByPageAll, log);
        }
        #endregion

        #region v2/custom_notification_events
        [FunctionName("CustomNotificationEvents")]
        public static async Task<IActionResult> CustomNotificationEvents([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/custom_notification_events/{idSpecie}/{page}/{totalByPage}/{desc?}")] HttpRequest req, string idSpecie, int page, int totalByPage, string desc, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var orderDate = string.IsNullOrWhiteSpace(desc) || desc.ToLower().Equals("desc");
            if (!orderDate && !desc.ToLower().Equals("asc"))
                return new BadRequestResult();
            var manager = await ContainerMethods.AgroManager(claims);
            var result = await manager.CustomManager.MobileEvents.GetNotificationPreOrdersResult(idSpecie, page, totalByPage, orderDate);
            return ContainerMethods.GetJsonGetContainer(result, log);
        }
        #endregion

        #region v2/notification/barrack/{idBarrack}
        [FunctionName("CustomNotificationBarrack")]
        public static async Task<IActionResult> CustomNotificationBarrack([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/notification/barrack/{idBarrack}")] HttpRequest req, string idBarrack, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            return ContainerMethods.GetJsonGetContainer(await manager.NotificationEvents.GetEventsByBarrackId(idBarrack), log);
        }
        #endregion

        #region v2/notification/barrack/{idBarrack}/phenological/{idPhenological}
        [FunctionName("CustomNotificationBarrackPhenologicalEvent")]
        public static async Task<IActionResult> CustomNotificationBarrackPhenologicalEvent([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/notification/barrack/{idBarrack}/phenological/{idPhenological}")] HttpRequest req, string idBarrack, string idPhenological, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            return ContainerMethods.GetJsonGetContainer(await manager.NotificationEvents.GetEventsByBarrackPhenologicalEventId(idBarrack, idPhenological), log);
        }
        #endregion

        #region v2/notification_events
        //[FunctionName("NotificationEvents")]
        //public static async Task<IActionResult> NotificationEvents([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v2/notification_events/{id?}")] HttpRequest req, string id,ILogger log){
        //    ClaimsPrincipal claims = await Auth.Validate(req);
        //    if (claims == null)
        //        return new UnauthorizedResult();
        //    var manager = await ContainerMethods.AgroManager(claims);
        //    Email email = new Email(manager.Users.GetUsers().Result.Result);
        //    //HttpClient client = new HttpClient();
        //    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("accessToken", EnvironmentVariableTarget.Process));
        //    //var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    //dynamic body = JsonConvert.DeserializeObject(requestBody);
        //    //var newBody = body["_parts"][0][1];
        //    //byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(newBody));
        //    //MemoryStream stream = new MemoryStream(byteArray);
        //    //var inputData = new StreamContent(stream);
        //    //string ipNgrok = Environment.GetEnvironmentVariable("ipNgrok", EnvironmentVariableTarget.Process);
        //    //await client.PostAsync("https://" + ipNgrok + ".ngrok.io/api/v2/debugroute", inputData);
        //    //client.Dispose();
        //    //return null;
        //    ExtGetContainer<NotificationEvent> result = null;
        //    switch (req.Method.ToLower()) {
        //        case "get":
        //            if (!string.IsNullOrWhiteSpace(id)) {
        //                switch (id) {
        //                    case "init":
        //                        var resultEvent = await manager.CustomManager.MobileEvents.GetEventData();
        //                        return ContainerMethods.GetJsonGetContainer(resultEvent, log);
        //                    case "ts":
        //                        var resultTs = await manager.CustomManager.MobileEvents.GetMobileEventTimestamp();
        //                        return ContainerMethods.GetJsonGetContainer(resultTs, log);
        //                    default:
        //                        result = await manager.NotificationEvents.GetEvent(id);
        //                        return ContainerMethods.GetJsonGetContainer(result, log);
        //                }
        //            }
        //            break;
        //        case "post":
        //            return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
        //                var newModel = model["_parts"][0][1];
        //                var idPhenologicalEvent = (string)newModel["idPhenologicalEvent"];
        //                var description = (string)newModel["description"];
        //                var base64 = (string)newModel["base64"];
        //                var barrack = (string)newModel["idBarrack"];
        //                //var lat = (float)newModel["latitude"];
        //                //var lon = (float)newModel["longitude"];
        //                var response = await db.NotificationEvents.SaveNewNotificationEvent(barrack, idPhenologicalEvent, base64, description, 0F, 0F);
        //                var evt = await db.NotificationEvents.GetEvent(response.IdRelated);
        //                var url = evt.Result.PicturePath;
        //                email.SendEmail("Notificacion",
        //                    $@"<html>
        //                    <body>
        //                        <p> Estimado(a), </p>
        //                        <p> Llego una notificacion </p>
        //                        <img src='{url}' style='width:50%;height:auto;'>
        //                        <p> Atentamente,<br> -Aresa </br></p>
        //                    </body>
        //                </html>");
        //                return response;
        //            }, claims);
        //    }
        //    ExtGetContainer<List<NotificationEvent>> resultGetAll = await manager.NotificationEvents.GetEvents();
        //    return ContainerMethods.GetJsonGetContainer(resultGetAll, log);
        //}
        #endregion

        [FunctionName("FiltersToBarracks")]
        public static async Task<IActionResult> FiltersToBarracks([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/filtersToBarracks/{idSector?}/{idSpecie?}/{idVariety?}")] HttpRequest req, string idSector, string idSpecie, string idVariety, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            List<Barrack> barracks;
            if (string.IsNullOrEmpty(idSector)) {
                List<Sector> sectors = manager.Sectors.GetSectors().Result.Result;
                return ContainerMethods.GetJsonGetContainer(OperationHelper.GetElement(sectors.Select(sector => new OutputMobileEntity { Id = sector.Id, EntityName = sector.CosmosEntityName, Name = sector.Name })), log);
            }
            else {
                barracks = manager.Barracks.GetBarracks().Result.Result.Where(barrack => barrack.PlotLand.Sector.Id.Equals(idSector) && barrack.SeasonId.Equals(manager.IdSeason)).ToList();
                if (string.IsNullOrEmpty(idSpecie)) {
                    List<Specie> species = barracks.Select(barrack => barrack.Variety.Specie).GroupBy(specie => specie.Name).Select(specie => specie.First()).ToList();
                    return ContainerMethods.GetJsonGetContainer(OperationHelper.GetElement(species.Select(specie => new OutputMobileEntity { Id = specie.Id, EntityName = specie.CosmosEntityName, Name = specie.Name })), log);
                }
                else {
                    barracks = barracks.Where(barrack => barrack.Variety.Specie.Id.Equals(idSpecie)).ToList();
                    if (string.IsNullOrEmpty(idVariety)) {
                        List<Variety> varieties = barracks.Select(barrack => barrack.Variety).GroupBy(variety => variety.Name).Select(variety => variety.First()).ToList();
                        return ContainerMethods.GetJsonGetContainer(OperationHelper.GetElement(varieties.Select(variety => new OutputMobileEntity { Id = variety.Id, EntityName = variety.CosmosEntityName, Name = variety.Name })), log);
                    }
                    else {
                        barracks = barracks.Where(barrack => barrack.Variety.Id.Equals(idVariety)).ToList();
                        return ContainerMethods.GetJsonGetContainer(OperationHelper.GetElement(barracks.Select(barrack => new OutputMobileEntity { Id = barrack.Id, EntityName = barrack.CosmosEntityName, Name = barrack.Name })), log);
                    }
                }
            }
        }

        [FunctionName("NotificationEvents")]
        public static async Task<IActionResult> NotificationEvents([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v2/notificationEvents/{idNotificationEvent?}")] HttpRequest req, string idNotificationEvent, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(idNotificationEvent)) {
                        var result = await manager.NotificationEvents.GetEvent(idNotificationEvent);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    else {
                        var resultAll = await manager.NotificationEvents.GetEvents();
                        return ContainerMethods.GetJsonGetContainer(resultAll, log);
                    }
                case "post":
                    Email email = new Email(manager.Users.GetUsers().Result.Result);
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var idPhenologicalEvent = (string)model["idPhenologicalEvent"];
                        var eventType = (NotificationType)model["eventType"];
                        var barrack = (string)model["idBarrack"];
                        var base64 = (string)model["base64"];
                        var description = (string)model["description"];
                        //var lat = (float)newModel["latitude"];
                        //var lon = (float)newModel["longitude"];
                        var response = await db.NotificationEvents.SaveNewNotificationEvent(idPhenologicalEvent, eventType, barrack, base64, description, 0F, 0F);
                        var evt = await db.NotificationEvents.GetEvent(response.IdRelated);
                        var url = evt.Result.PicturePath;
                        email.SendEmail("Notificacion",
                            $@"<html>
                                <body>
                                    <p> Estimado(a), </p>
                                    <p> Llego una notificacion </p>
                                    <img src='{url}' style='width:50%;height:auto;'>
                                    <p> Atentamente,<br> -Aresa </br></p>
                                </body>
                            </html>");
                        return response;
                    }, claims);
            }
            return new BadRequestResult();
        }



        [FunctionName("searchProducts")]
        public static async Task<IActionResult> SearchProducts([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/search/products")] HttpRequest req, ILogger log)
        {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();


            var manager = await ContainerMethods.AgroManager(claims);

            var products = manager.Products.GetIndexElements("", null, null, null);


            return ContainerMethods.GetJsonGetContainer(new ExtGetContainer<List<Element>> { 
                StatusResult = ExtGetDataResult.Success,
                Result = products.Result.Entities.Select(s => new Element { Id = s.Id, Name = s.Name, Created = s.Created }).ToList()
            }, log);

        }


        [FunctionName("IndexElementsFilter")]
        public static async Task<IActionResult> IndexElementsFilter([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/{entityName}/indexElements/{abbSpecie?}/{type?}/{status?}/{textToSearch?}/{asc?}/{totalByPage?}/{page?}")] HttpRequest req, string entityName, string abbSpecie, string type, string status, string textToSearch, string asc, int? totalByPage, int? page, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            bool? isPhenological = null;
            if (!string.IsNullOrWhiteSpace(type) && !type.ToLower().Equals("all"))
            {
                if (type.ToLower().Equals("phenological")) isPhenological = true;
                else if (type.ToLower().Equals("not_phenological")) isPhenological = false;
                else return new BadRequestResult();
            }
            int? statusToFilter = null;
            if (!string.IsNullOrWhiteSpace(status) && !status.ToLower().Equals("all")) {
                if (int.TryParse(status, out int outResult)) statusToFilter = outResult;
                else return new BadRequestResult();
            }
            if (!string.IsNullOrWhiteSpace(textToSearch) && textToSearch.Equals("*"))
                textToSearch = null;
            bool? order = null;
            if (!string.IsNullOrWhiteSpace(asc) && !asc.ToLower().Equals("not_order"))
            {
                if (asc.ToLower().Equals("asc")) order = true;
                else if (asc.ToLower().Equals("desc")) order = false;
                else return new BadRequestResult();
            }
            if (!string.IsNullOrWhiteSpace(abbSpecie) && abbSpecie.ToLower().Equals("all"))
                abbSpecie = null;
            ExtGetContainer<EntitiesSearchContainer> result;
            switch (entityName.ToLower()) {
                case "orders":
                    result = manager.ApplicationOrders.GetIndexElements(textToSearch, abbSpecie, isPhenological, page, totalByPage, order);
                    break;
                case "executions":
                    result = manager.ExecutionOrders.GetIndexElements(textToSearch, abbSpecie, statusToFilter, page, totalByPage, order);
                    break;
                case "products":
                    result = manager.Products.GetIndexElements(textToSearch, page, totalByPage, order);
                    break;
                case "barracks":
                    result = manager.Barracks.GetIndexElements(textToSearch, abbSpecie, page, totalByPage, order);
                    break;
                default:
                    return ContainerMethods.GetJsonGetContainer(OperationHelper.GetException<ExecutionOrder>(new Exception($"No existe entityName: {entityName}")), log);
            }
            return ContainerMethods.GetJsonGetContainer(result, log);


        }

        #region v2/orders
        [FunctionName("Orders")]
        public static async Task<IActionResult> Orders([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/orders/{idOrder?}/{abbSpecie?}/{type?}/{textToSearch?}/{asc?}/{totalByPage?}/{page?}")] HttpRequest req, string idOrder, string abbSpecie, string type, string textToSearch, string asc, int? totalByPage, int? page, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<OutPutApplicationOrder> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(idOrder) && !idOrder.ToLower().Equals("all")) {
                        result = await manager.ApplicationOrders.GetApplicationOrder(idOrder);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations<string>(req.Body, log, async (db, model) => {
                        var input = JsonConvert.DeserializeObject<ApplicationOrderInput>(model.ToString());
                        return await db.ApplicationOrders.SaveNewApplicationOrder(input);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations<OutPutApplicationOrder>(req.Body, log, async (db, model) => {
                        var input = JsonConvert.DeserializeObject<ApplicationOrderInput>(model.ToString());
                        return await db.ApplicationOrders.SaveEditApplicationOrder(idOrder, input);
                    }, claims);
            }
            if (!string.IsNullOrWhiteSpace(textToSearch) && textToSearch.Equals("*"))
                textToSearch = null;
            if (!string.IsNullOrWhiteSpace(abbSpecie) && abbSpecie.ToLower().Equals("all"))
                abbSpecie = null;
            else abbSpecie = abbSpecie.ToUpper();
            bool? isPhenological = null;
            if (!string.IsNullOrWhiteSpace(type) && !asc.ToLower().Equals("all")) {
                if (type.ToLower().Equals("phenological")) isPhenological = true;
                else if (type.ToLower().Equals("not_phenological")) isPhenological = false;
                else return new BadRequestResult();
            }
            bool? order = null;
            if (!string.IsNullOrWhiteSpace(asc) && !asc.ToLower().Equals("not_order")) {
                if (asc.ToLower().Equals("asc")) order = true;
                else if (asc.ToLower().Equals("desc")) order = false;
                else return new BadRequestResult();
            }
            var resultGetByPageAll = manager.ApplicationOrders.GetPaginatedOrders(textToSearch, abbSpecie, isPhenological, page, totalByPage, order);
            return ContainerMethods.GetJsonGetContainer(resultGetByPageAll, log);
        }
        #endregion

        #region v2/ordersByExecutionStatus
        [FunctionName("OrderByExecutionStatus")]
        public static async Task<IActionResult> OrderByExecutionStatus([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/orders/getByExecutionStatus/{status}")] HttpRequest req, int status, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<List<ExecutionOrder>> resultGetByStatus = await manager.ExecutionOrders.GetExecutionOrders();
            resultGetByStatus.Result = resultGetByStatus.Result.Where(execution => execution.ExecutionStatus == (ExecutionStatus)status).ToList();
            ExtGetContainer<List<OutPutApplicationOrder>> resultGetAll = await manager.ApplicationOrders.GetApplicationOrders();
            resultGetAll.Result = resultGetAll.Result.Where(order => resultGetByStatus.Result.Any(execution => execution.Order.Id.Equals(order.Id))).ToList();
            return ContainerMethods.GetJsonGetContainer(resultGetAll, log);
        }
        #endregion

        #region v2/sectors
        [FunctionName("SectorV2")]
        public static async Task<IActionResult> Sector([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/sectors/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<Sector> result = null;
            switch (req.Method.ToLower())
            {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        result = await manager.Sectors.GetSector(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        return await db.Sectors.SaveNewSector(name);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        return await db.Sectors.SaveEditSector(id, name);
                    }, claims);
            }
            ExtGetContainer<List<Sector>> resultGetAll = await manager.Sectors.GetSectors();
            return ContainerMethods.GetJsonGetContainer(resultGetAll, log);
        }
        #endregion

        #region v2/plotlands
        [FunctionName("PlotLandsV2")]
        public static async Task<IActionResult> PlotLandsV2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/plotlands/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<PlotLand> result = null;
            switch (req.Method.ToLower())
            {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        result = await manager.PlotLands.GetPlotLand(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var idSector = (string)model["idSector"];
                        return await db.PlotLands.SaveNewPlotLand(name, idSector);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var idSector = (string)model["idSector"];
                        return await db.PlotLands.SaveEditPlotLand(id, name, idSector);
                    }, claims);
            }
            ExtGetContainer<List<PlotLand>> resultGetAll = await manager.PlotLands.GetPlotLands();
            return ContainerMethods.GetJsonGetContainer(resultGetAll, log);
        }
        #endregion

        #region v2/varieties
        [FunctionName("VarietiesV2")]
        public static async Task<IActionResult> VarietiesV2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/varieties/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<Variety> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id)) {
                        result = await manager.Varieties.GetVariety(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var name = (string)model["name"];
                        var idSpecie = (string)model["idSpecie"];
                        var abbreviation = (string)model["abbreviation"];
                        return await db.Varieties.SaveNewVariety(name, abbreviation, idSpecie);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var name = (string)model["name"];
                        var idSpecie = (string)model["idSpecie"];
                        var abbreviation = (string)model["abbreviation"];
                        return await db.Varieties.SaveEditVariety(id, name, abbreviation, idSpecie);
                    }, claims);
            }
            ExtGetContainer<List<Variety>> resultGetAll = await manager.Varieties.GetVarieties();
            return ContainerMethods.GetJsonGetContainer(resultGetAll, log);
        }
        #endregion

        #region v2/barracks
        [FunctionName("BarracksV2")]
        public static async Task<IActionResult> BarracksV2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/barracks/{idBarrack?}/{abbSpecie?}/{textToSearch?}/{asc?}/{totalByPage?}/{page?}")] HttpRequest req, string idBarrack, string abbSpecie, string textToSearch, string asc, int? totalByPage, int? page, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<Barrack> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(idBarrack) && !idBarrack.ToLower().Equals("all")) {
                        result = await manager.Barracks.GetBarrack(idBarrack);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var name = (string)model["name"];
                        var numberOfPlants = (int)model["numberOfPlants"];
                        var plantingYear = (int)model["plantingYear"];
                        var hectares = float.Parse((string)model["hectares"]);
                        var idPlotland = (string)model["idPlotland"];
                        var idVariety = (string)model["idVariety"];
                        var idPollinator = (string)model["idPollinator"];
                        var idRootstock = (string)model["idRootstock"];
                        return await db.Barracks.SaveNewBarrack(name, idPlotland, hectares, plantingYear, idVariety, numberOfPlants, idPollinator, idRootstock);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var name = (string)model["name"];
                        var numberOfPlants = (int)model["numberOfPlants"];
                        var plantingYear = (int)model["plantingYear"];
                        var hectares = float.Parse((string)model["hectares"]);
                        var idPlotland = (string)model["idPlotland"];
                        var idVariety = (string)model["idVariety"];
                        var idPollinator = (string)model["idPollinator"];
                        var idRootstock = (string)model["idRootstock"];
                        return await db.Barracks.SaveEditBarrack(idBarrack, name, idPlotland, hectares, plantingYear, idVariety, numberOfPlants, idPollinator, idRootstock);
                    }, claims);
            }
            if (!string.IsNullOrWhiteSpace(textToSearch) && textToSearch.Equals("*"))
                textToSearch = null;
            if (!string.IsNullOrWhiteSpace(abbSpecie) && abbSpecie.ToLower().Equals("all"))
                abbSpecie = null;
            else abbSpecie = abbSpecie.ToUpper();
            bool? order = null;
            if (!string.IsNullOrWhiteSpace(asc) && !asc.ToLower().Equals("not_order")) {
                if (asc.ToLower().Equals("asc")) order = true;
                else if (asc.ToLower().Equals("desc")) order = false;
                else return new BadRequestResult();
            }
            var resultGetByPageAll = manager.Barracks.GetPaginatedBarracks(textToSearch, abbSpecie, page, totalByPage, order);
            return ContainerMethods.GetJsonGetContainer(resultGetByPageAll, log);
        }
        #endregion

        #region v2/phenological_preorders
        [FunctionName("PhenologicalPreOrders")]
        public static async Task<IActionResult> PhenologicalPreOrders([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/phenological_preorders/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<PhenologicalPreOrder> result = null;
            switch (req.Method.ToLower())
            {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        result = await manager.PhenologicalPreOrders.GetPhenologicalPreOrder(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var idFolder = (string)model["idOrderFolder"];
                        var arr = (string)model["idBarracks"].ToString();
                        var idBarracks = JsonConvert.DeserializeObject<string[]>(arr);

                        return await db.PhenologicalPreOrders.SaveNewPhenologicalPreOrder(name, idFolder, idBarracks.ToList());
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        var idFolder = (string)model["idOrderFolder"];
                        var arr = (string)model["idBarracks"].ToString();
                        var idBarracks = JsonConvert.DeserializeObject<string[]>(arr);
                        return await db.PhenologicalPreOrders.SaveEditPhenologicalPreOrder(id, name, idFolder, idBarracks.ToList());
                    }, claims);
            }
            ExtGetContainer<List<PhenologicalPreOrder>> resultGetAll = await manager.PhenologicalPreOrders.GetPhenologicalPreOrders();
            return ContainerMethods.GetJsonGetContainer(resultGetAll, log);
        }
        #endregion

        #region v2/roles
        [FunctionName("Roles")]
        public static async Task<IActionResult> Roles([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/roles/{id?}")] HttpRequest req, ILogger log, string id) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id)) {
                        var result = await manager.Roles.GetRole(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    else {
                        var resultAll = await manager.Nebulizers.GetNebulizers();
                        return ContainerMethods.GetJsonGetContainer(resultAll, log);
                    }
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        return await db.Roles.SaveNewRole(name);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        return await db.Roles.SaveEditRole(id, name);
                    }, claims);
            }
            return new BadRequestResult();
        }
        #endregion

        #region v2/jobs
        [FunctionName("Jobs")]
        public static async Task<IActionResult> Jobs([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/jobs/{id?}")] HttpRequest req, ILogger log, string id) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<List<Job>> result = null;
            switch (req.Method.ToLower())
            {
                case "get":
                    result = await manager.Jobs.GetJobs();
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        return await db.Jobs.SaveNewJob(name);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) =>
                    {
                        var name = (string)model["name"];
                        return await db.Jobs.SaveEditJob(id, name);
                    }, claims);
            }
            return ContainerMethods.GetJsonGetContainer(result, log);
        }
        #endregion

        #region v2/users
        [FunctionName("Users")]
        public static async Task<IActionResult> Users([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/users/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<UserApplicator> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id)) {
                        result = await manager.Users.GetUser(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var name = (string)model?["name"];
                        var rut = (string)model?["rut"];
                        var email = (string)model?["email"];
                        var idJob = (string)model?["idJob"];
                        var idsRoles = (string[])model?["idsRoles"]?.ToObject<string[]>();
                        var idTractor = (string)model?["idTractor"];
                        var idNebulizer = (string)model?["idNebulizer"];
                        return await db.Users.SaveNewUser(name, rut, email, idJob, idsRoles, idNebulizer, idTractor);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var name = (string)model["name"];
                        var rut = (string)model["rut"];
                        var email = (string)model["email"];
                        var idJob = (string)model["idJob"];
                        var idsRoles = (string[])model["idsRoles"].ToObject<string[]>();
                        var idTractor = (string)model["idTractor"];
                        var idNebulizer = (string)model["idNebulizer"];
                        return await db.Users.SaveEditUser(id, name, rut, email, idJob, idsRoles, idNebulizer, idTractor);
                    }, claims);
            }
            ExtGetContainer<List<UserApplicator>> resultGetAll = await manager.Users.GetUsers();
            return ContainerMethods.GetJsonGetContainer(resultGetAll, log);
        }
        #endregion

        #region v2/usersByRole
        [FunctionName("UserByRole")]
        public static async Task<IActionResult> UserByRole([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/users/getByRole/{roleName}")] HttpRequest req, string roleName, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<List<UserApplicator>> resultGetAllByRole = await manager.Users.GetUsers();
            resultGetAllByRole.Result = resultGetAllByRole.Result.Where(user => user.Roles.Any(role => role.Name.Equals(roleName))).ToList();
            return ContainerMethods.GetJsonGetContainer(resultGetAllByRole, log);
        }
        #endregion

        #region v2/userInfo
        [FunctionName("UserInfo")]
        public static async Task<IActionResult> UsersRoles([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/userinfo")] HttpRequest req, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<UserApplicator> result = await manager.Users.GetUserFromToken();
            return ContainerMethods.GetJsonGetContainer(result, log);
        }
        #endregion

        #region v2/nebulizers
        [FunctionName("Nebulizers")]
        public static async Task<IActionResult> Nebulizers([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/nebulizers/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id)) {
                        var result = await manager.Nebulizers.GetNebulizer(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    else {
                        var resultAll = await manager.Nebulizers.GetNebulizers();
                        return ContainerMethods.GetJsonGetContainer(resultAll, log);
                    }
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var brand = (string)model["brand"];
                        var code = (string)model["code"];
                        return await db.Nebulizers.SaveNewNebulizer(brand, code);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var brand = (string)model["brand"];
                        var code = (string)model["code"];
                        return await db.Nebulizers.SaveEditNebulizer(id, brand, code);
                    }, claims);
            }
            return new BadRequestResult();
        }
        #endregion

        #region v2/tractors
        [FunctionName("Tractors")]
        public static async Task<IActionResult> Tractors([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/tractors/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id)) {
                        var result = await manager.Tractors.GetTractor(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    else {
                        var resultAll = await manager.Tractors.GetTractors();
                        return ContainerMethods.GetJsonGetContainer(resultAll, log);
                    }
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var brand = (string)model["brand"];
                        var code = (string)model["code"];
                        return await db.Tractors.SaveNewTractor(brand, code);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var brand = (string)model["brand"];
                        var code = (string)model["code"];
                        return await db.Tractors.SaveEditTractor(id, brand, code);
                    }, claims);
            }
            return new BadRequestResult();
        }
        #endregion

        #region v2/executions
        [FunctionName("Executions")]
        public static async Task<IActionResult> Executions([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/executions/{idExecution?}/{abbSpecie?}/{status?}/{textToSearch?}/{asc?}/{totalByPage?}/{page?}")] HttpRequest req, string idExecution, string abbSpecie, string status, string textToSearch, string asc, int? totalByPage, int? page, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            ExtGetContainer<ExecutionOrder> result = null;
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(idExecution) && !idExecution.ToLower().Equals("all")) {
                        result = await manager.ExecutionOrders.GetExecutionOrder(idExecution);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    break;
                case "post":
                    return await ContainerMethods.ApiPostOperations<string>(req.Body, log, async (db, model) => {
                        string idOrder = (string)model["idOrder"];
                        string idUserApplicator = (string)model["idUserApplicator"];
                        string idNebulizer = (string)model["idNebulizer"];
                        string[] idProduct = JsonConvert.DeserializeObject<string[]>(((object)model["idProduct"]).ToString());
                        double[] quantityByHectare = JsonConvert.DeserializeObject<double[]>(((object)model["quantityByHectare"]).ToString());
                        string idTractor = (string)model["idTractor"];
                        string commentary = (string)model["commentary"];
                        string executionName = (string)model["executionName"];
                        return await db.ExecutionOrders.SaveNewExecutionOrder(idOrder, executionName, idUserApplicator, idNebulizer, idProduct, quantityByHectare, idTractor, commentary);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations<ExecutionOrder>(req.Body, log, async (db, model) => {
                        string idOrder = (string)model["idOrder"];
                        string idUserApplicator = (string)model["idUserApplicator"];
                        string idNebulizer = (string)model["idNebulizer"];
                        string[] idProduct = JsonConvert.DeserializeObject<string[]>(((object)model["idProduct"]).ToString());
                        double[] quantityByHectare = JsonConvert.DeserializeObject<double[]>(((object)model["quantityByHectare"]).ToString());
                        string idTractor = (string)model["idTractor"];
                        string executionName = (string)model["executionName"];
                        return await db.ExecutionOrders.SaveEditExecutionOrder(idExecution, idOrder, executionName, idUserApplicator, idNebulizer, idProduct, quantityByHectare, idTractor);
                    }, claims);
            }
            if (!string.IsNullOrWhiteSpace(textToSearch) && textToSearch.Equals("*"))
                textToSearch = null;
            if (!string.IsNullOrWhiteSpace(abbSpecie) && abbSpecie.ToLower().Equals("all"))
                abbSpecie = null;
            else abbSpecie = abbSpecie.ToUpper();
            int? statusToFilter = null;
            if (!string.IsNullOrWhiteSpace(status) && !status.ToLower().Equals("all")) {
                if (int.TryParse(status, out int outResult)) statusToFilter = outResult;
                else return new BadRequestResult();
            }
            bool? order = null;
            if (!string.IsNullOrWhiteSpace(asc) && !asc.ToLower().Equals("not_order")) {
                if (asc.ToLower().Equals("asc")) order = true;
                else if (asc.ToLower().Equals("desc")) order = false;
                else return new BadRequestResult();
            }
            var resultGetByPageAll = manager.ExecutionOrders.GetPaginatedExecutions(textToSearch, abbSpecie, statusToFilter, page, totalByPage, order);
            return ContainerMethods.GetJsonGetContainer(resultGetByPageAll, log);
        }
        #endregion

        #region v2/Executions_ChangeStatus
        [FunctionName("Execution_ChangeStatus")]
        public static async Task<IActionResult> Execution_ChangeStatus([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v2/execution/changeStatus/{idExecution}")] HttpRequest req, string idExecution, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                var type = (string)model["type"];
                var value = (int)model["value"];
                var commentary = (string)model["commentary"];
                return await manager.ExecutionOrders.SetStatus(idExecution, type, value, commentary);
            }, claims);
        }
        #endregion

        #region v2/executionsAddCommentary
        [FunctionName("ExecutionsAddCommentary")]
        public static async Task<IActionResult> ExecutionsAddCommentary([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v2/execution/add_commentary/{idExecution}")] HttpRequest req, string idExecution, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            return await ContainerMethods.ApiPostOperations<ExecutionOrder>(req.Body, log, async (db, model) => {
                string commentary = (string)model["commentary"];
                return await db.ExecutionOrders.AddCommentaryToExecutionOrder(idExecution, commentary);
            }, claims);
        }
        #endregion

        #region v2/businessName
        [FunctionName("BusinessName")]
        public static async Task<IActionResult> BusinessName([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/businessNames/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id)) {
                        var result = await manager.BusinessNames.GetBusinessName(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    else {
                        var resultAll = await manager.BusinessNames.GetBusinessNames();
                        return ContainerMethods.GetJsonGetContainer(resultAll, log);
                    }
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var name = (string)model["name"];
                        var rut = (string)model["rut"];
                        var phone = (string)model["phone"];
                        var email = (string)model["email"];
                        var webPage = (string)model["webPage"];
                        var giro = (string)model["giro"];
                        return await db.BusinessNames.SaveNewBusinessName(name,rut,phone,email,webPage,giro);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var name = (string)model["name"];
                        var rut = (string)model["rut"];
                        var phone = (string)model["phone"];
                        var email = (string)model["email"];
                        var webPage = (string)model["webPage"];
                        var giro = (string)model["giro"];
                        return await db.BusinessNames.SaveEditBusinessName(id, name, rut, phone, email, webPage, giro);
                    }, claims);
            }
            return new BadRequestResult();
        }
        #endregion
        #region v2/costCenter
        [FunctionName("CostCenter")]
        public static async Task<IActionResult> CostCenter([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/costCenters/{id?}")] HttpRequest req, string id, ILogger log) {
            ClaimsPrincipal claims = await Auth.Validate(req);
            if (claims == null)
                return new UnauthorizedResult();
            var manager = await ContainerMethods.AgroManager(claims);
            switch (req.Method.ToLower()) {
                case "get":
                    if (!string.IsNullOrWhiteSpace(id)) {
                        var result = await manager.CostCenters.GetCostCenter(id);
                        return ContainerMethods.GetJsonGetContainer(result, log);
                    }
                    else {
                        var resultAll = await manager.CostCenters.GetCostCenters();
                        return ContainerMethods.GetJsonGetContainer(resultAll, log);
                    }
                case "post":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var name = (string)model["name"];
                        var idRazonSocial = (string)model["idRazonSocial"];
                        return await db.CostCenters.SaveNewCostCenter(name, idRazonSocial);
                    }, claims);
                case "put":
                    return await ContainerMethods.ApiPostOperations(req.Body, log, async (db, model) => {
                        var name = (string)model["name"];
                        var idRazonSocial = (string)model["idRazonSocial"];
                        return await db.CostCenters.SaveEditCostCenter(id, name, idRazonSocial);
                    }, claims);
            }
            return new BadRequestResult();
        }
        #endregion


        #region Login
        [FunctionName("login")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log) {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic credenciales = JsonConvert.DeserializeObject(requestBody);

            string clientId = "a81f0ad4-912b-46d3-ba3e-7bf605693242";
            string scope = "https://sprint3-jhm.trifenix.io/App.access";
            string clientSecret = "gUjIa4F=NXlAwwMCF2j2SFMMj3?m@=FM";
            string username = (string)credenciales["username"];
            string password = (string)credenciales["password"];
            string grantType = "password";
            string endPoint = "https://login.microsoftonline.com/jhmad.onmicrosoft.com/oauth2/v2.0/token";

            HttpClient client = new HttpClient();
            var parametros = new Dictionary<string, string> {
                {"client_id",clientId},
                {"scope",scope},
                {"client_secret",clientSecret},
                {"username",username},
                {"password",password},
                {"grant_type",grantType}
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endPoint);
            requestMessage.Content = new FormUrlEncodedContent(parametros);
            var response = await client.SendAsync(requestMessage);
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(responseBody);
            client.Dispose();
            string accessToken = json.access_token;
            return ContainerMethods.GetJsonGetContainer(OperationHelper.GetElement(accessToken), log);
        }
        #endregion

        [FunctionName("DebugRoute")]
        public static async Task<IActionResult> DebugRoute([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "v2/debugroute/{id?}")] HttpRequest req, ILogger log, string id){
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic result = JsonConvert.DeserializeObject(requestBody);
            return result;
        }

    }
}