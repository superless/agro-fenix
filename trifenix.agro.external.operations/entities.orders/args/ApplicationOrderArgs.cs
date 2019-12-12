﻿using trifenix.agro.db.interfaces.agro;
using trifenix.agro.db.interfaces.agro.events;
using trifenix.agro.db.interfaces.agro.ext;
using trifenix.agro.db.interfaces.agro.fields;
using trifenix.agro.db.interfaces.agro.orders;
using trifenix.agro.db.interfaces.common;
using trifenix.agro.db.model.agro.orders;
using trifenix.agro.microsoftgraph.interfaces;

namespace trifenix.agro.external.operations.entities.orders.args
{
    public class ApplicationOrderArgs
    {
        public DosesArgs DosesArgs { get; set; }

        public ApplicationOrderCommonDbArgs CommonDb { get; set; }

        public IBarrackRepository Barracks { get; set; }

        public IPhenologicalPreOrderRepository PreOrder { get; set; }

        public IApplicationOrderRepository ApplicationOrder { get; set; }

        public IGraphApi GraphApi { get; set; }

        public IProductRepository Product { get; set; }

        public INotificationEventRepository Notifications { get; set; }


        public string SeasonId { get; set; }




    }


    public class ApplicationOrderCommonDbArgs {
        public ICommonDbOperations<ApplicationOrder> ApplicationOrder { get; set; }

    } 

    public class DosesArgs {
        

        public IApplicationTargetRepository Target { get; set; }

        public ISpecieRepository Specie { get; set; }

        public IVarietyRepository Variety { get; set; }

        public ICertifiedEntityRepository CertifiedEntity { get; set; }


    }
}
