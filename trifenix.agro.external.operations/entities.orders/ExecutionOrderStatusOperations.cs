﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using trifenix.agro.db.interfaces;
using trifenix.agro.db.interfaces.agro.common;
using trifenix.agro.db.model.agro.orders;
using trifenix.agro.enums;
using trifenix.agro.external.interfaces;
using trifenix.agro.model.external;
using trifenix.agro.model.external.Input;
using trifenix.agro.search.interfaces;
using trifenix.agro.search.model;

namespace trifenix.agro.external.operations.entities.orders
{
    public class ExecutionOrderStatusOperations : MainReadOperation<ExecutionOrderStatus>, IGenericOperation<ExecutionOrderStatus, ExecutionOrderStatusInput>
    {
        public ExecutionOrderStatusOperations(IMainGenericDb<ExecutionOrderStatus> repo, IExistElement existElement, IAgroSearch search) : base(repo, existElement, search)
        {
        }

        private async Task<string> ValidaExecutionStatus(ExecutionOrderStatusInput input) {
            if (!string.IsNullOrWhiteSpace(input.Id))
            {
                var exists = await existElement.ExistsElement<ExecutionOrder>(input.Id);
                if (!exists) return $"la estado de ejecución con id {input.Id} no existe";
            }

            var existsExecution = await existElement.ExistsElement<ExecutionOrder>(input.IdExecutionOrder);

            if (!existsExecution) return $"no existe ejecución con id {input.IdExecutionOrder}";

            return string.Empty;

        }

        public async Task<ExtPostContainer<string>> Save(ExecutionOrderStatusInput input)
        {
            var id = !string.IsNullOrWhiteSpace(input.Id) ? input.Id : Guid.NewGuid().ToString("N");

            var validaExecutionStatus = await ValidaExecutionStatus(input);


            if (!string.IsNullOrWhiteSpace(validaExecutionStatus)) throw new Exception(validaExecutionStatus);

            var order = new ExecutionOrderStatus
            {
                Id = id,
                ClosedStatus = input.ClosedStatus,
                ExecutionStatus = input.ExecutionStatus,
                Comment = input.Comment,
                Created = input.Created,
                FinishStatus = input.FinishStatus,
                IdExecutionOrder = input.IdExecutionOrder
            };

            await repo.CreateUpdate(order);

            search.AddElements(new List<EntitySearch> {
                new EntitySearch { 
                    EntityIndex = (int)EntityRelated.EXECUTION_ORDER_STATUS,
                    RelatedIds = new RelatedId[]{ 
                        new RelatedId{  EntityIndex= (int)EntityRelated.EXECUTION_ORDER, EntityId = input.IdExecutionOrder}
                    },
                    Created = DateTime.Now,
                    Id = id,
                    RelatedProperties = new Property[]{ 
                        new Property{ PropertyIndex = (int)PropertyRelated.GENERIC_COMMENT, Value = input.Comment}
                    },
                    RelatedEnumValues = new RelatedEnumValue[]{ 
                        new RelatedEnumValue{ EnumerationIndex = (int)EnumerationRelated.EXECUTION_STATUS, Value = (int)input.ExecutionStatus  },
                        new RelatedEnumValue{ EnumerationIndex = (int)EnumerationRelated.EXECUTION_CLOSED_STATUS, Value = (int)input.ClosedStatus  },
                        new RelatedEnumValue{ EnumerationIndex = (int)EnumerationRelated.EXECUTION_FINISH_STATUS, Value = (int)input.FinishStatus  }
                    }
                }
            });

            return new ExtPostContainer<string>
            {
                IdRelated = id,
                MessageResult = ExtMessageResult.Ok,
                Result = id
            };
        }
    }
}

/*Ejecucion
* Anadir campos (producto,cantidad) relacionados a la orden.
* Es necesario crear una ruta para obtener lista de usuarios aplicadores
* Es necesario crear una ruta para obtener ejecuciones en proceso (Transversal a las ordenes)
* Es necesario crear una ruta para obtener ordenes que contengan ejecuciones en proceso
* Cuando la ejecucion cambie su executionStatus a 1:InProcess, se copiaran la fecha inicial y final de la orden a si misma.
* Cada vez que se setee el executionStatus (Inicialmente y sus sucesivos cambios), se debe almacenar la fecha (ExecutionStatusDate)
* El nuevo executionStatus debe ser siempre igual o superior al anterior (Como maximo en una unidad, ya que este estado es serial)
* EL finishStatus solo se puede setear cuando el executionStatus tiene el valor 2:EndProcess
* El closedStatus solo se puede setear cuando el executionStatus tiene el valor 3:Closed
* Al crear una ejecucion (En planificacion) es obligatoria la orden relacionada.
* Al iniciar la ejecucion (En proceso) el usuario aplicador asignado es obligatorio.
* El closedStatus solo puede ser seteado si el usuario posee el rol de "Administrador".
* Comentarios para cada estado, independiente de los comentarios transversales.
* Si la ejecucion ya finalizo(finishStatus != 0) solo se pueden recibir comentarios y cierre de ejecucion(set closedStatus to != 0)
* Si la orden relacionada ya posee una ejecucion exitosa no se puede crear una nueva ejecucion.*/
