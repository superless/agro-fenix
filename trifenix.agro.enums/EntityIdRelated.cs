﻿namespace trifenix.agro.enums {

    public enum EntityRelated {
        WAITINGHARVEST = 0,
        BARRACK = 1, 
        BUSINESSNAME = 2, // search
        CATEGORY_INGREDIENT = 3,
        CERTIFIED_ENTITY = 4, // search
        COSTCENTER = 5,
        DOSES = 6,
        INGREDIENT = 7,
        JOB = 8, // search
        NEBULIZER = 9,
        PHENOLOGICAL_EVENT = 10,
        PLOTLAND = 11, // search
        PRODUCT = 12,
        ROLE = 13,
        ROOTSTOCK = 14,
        SEASON = 15,
        SECTOR = 16, // sector
        PREORDER = 17,
        TARGET = 18,
        TRACTOR = 19,
        USER = 20, //user
        VARIETY = 21, // search
        NOTIFICATION_EVENT = 22,
        POLLINATOR = 23,
        ORDER_FOLDER = 24,
        EXECUTION_ORDER = 25,
        ORDER = 26,
        BARRACK_EVENT = 27,
        DOSES_ORDER = 28,
        EXECUTION_ORDER_STATUS = 29,
        SPECIE = 30, // search
        GEOPOINT = 31 // search
    }

    public enum PropertyRelated {
        GENERIC_GIRO = 0,
        GENERIC_ABBREVIATION = 1,
        GENERIC_BRAND = 2,
        GENERIC_CODE = 3,
        GENERIC_EMAIL = 4 ,
        GENERIC_END_DATE= 5,
        GENERIC_NAME = 6,
        GENERIC_RUT = 7,
        GENERIC_START_DATE = 8,
        GENERIC_DESC = 9,
        GENERIC_PATH = 10,
        GENERIC_QUANTITY_HECTARE = 11,
        GENERIC_COMMENT = 12,
        GENERIC_PHONE=13,
        GENERIC_NUMBER_OF_PLANTS=14,
        GENERIC_PLANT_IN_YEAR = 15,
        GENERIC_HECTARES = 16,
        GENERIC_LATITUDE = 17,
        GENERIC_LONGITUDE = 18,
        GENERIC_WETTING = 19,
        OBJECT_ID_AAD = 20,
        GENERIC_WEBPAGE = 21,
        GENERIC_QUANTITY = 22,
        DOSES_HOURSENTRYBARRACK= 23,
        DOSES_DAYSINTERVAL = 24,
        DOSES_SEQUENCE = 25,
        DOSES_WETTINGRECOMMENDED = 26,        
        DOSES_WAITINGDAYSLABEL = 27,
        DOSES_WAITINGHARVESTDAYS = 28,
        GENERIC_COUNTER = 29,
        WAITINGHARVEST_DAYS = 30,
        WAITINGHARVEST_PPM = 31
    }

    public enum EnumerationRelated {
        ORDER_TYPE= 1,
        SEASON_CURRENT=2, //0 false. 1 true
        NOTIFICATION_TYPE = 3,
        PREORDER_TYPE = 4,
        EXECUTION_STATUS = 5,
        EXECUTION_CLOSED_STATUS = 6,
        EXECUTION_FINISH_STATUS = 7,
        PRODUCT_KINDOFBOTTLE = 8,
        PRODUCT_MEASURETYPE = 9,
        GENERIC_ACTIVE = 10, //0 false, 1 true,
        GENERIC_DEFAULT = 11, //0 false, 1 true
        DOSES_DOSESAPPLICATEDTO = 12,
        
    }

    public enum OrderType { 
        DEFAULT = 0,
        PHENOLOGICAL = 1
    }

    public enum PreOrderType {
        DEFAULT = 0,
        PHENOLOGICAL = 1
    }

    public enum CurrentSeason {
        NOT_CURRENT = 0,
        CURRENT = 1
    }

}