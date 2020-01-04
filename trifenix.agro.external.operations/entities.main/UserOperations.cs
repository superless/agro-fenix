﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using trifenix.agro.db.interfaces.agro;
using trifenix.agro.db.interfaces.common;
using trifenix.agro.db.model.agro;
using trifenix.agro.external.interfaces.entities.main;
using trifenix.agro.external.operations.helper;
using trifenix.agro.microsoftgraph.interfaces;
using trifenix.agro.model.external;

namespace trifenix.agro.external.operations.entities.main
{
    public class UserOperations : IUserOperations {
        private readonly IUserRepository _repo;
        private readonly IGraphApi _graphApi;
        private readonly IJobRepository _repoJob;
        private readonly IRoleRepository _repoRole;
        private readonly INebulizerRepository _repoNebulizer;
        private readonly ITractorRepository _repoTractor;
        private readonly ICommonDbOperations<UserApplicator> _commonDb;
        public UserOperations(IUserRepository repo, IGraphApi graphApi, IJobRepository repoJob, IRoleRepository repoRole, INebulizerRepository repoNebulizer, ITractorRepository repoTractor, ICommonDbOperations<UserApplicator> commonDb)
        {
            _repo = repo;
            _graphApi = graphApi;
            _repoJob = repoJob;
            _repoRole = repoRole;
            _repoNebulizer = repoNebulizer;
            _repoTractor = repoTractor;
            _commonDb = commonDb;
        }

        public async Task<ExtGetContainer<UserApplicator>> GetUser(string id)
        {
            var user = await _repo.GetUser(id);
            return OperationHelper.GetElement(user);
        }

        public async Task<ExtGetContainer<UserApplicator>> GetUserFromToken()
        {
            var user = await _graphApi.GetUserFromToken();
            return OperationHelper.GetElement(user);
        }

        public async Task<ExtGetContainer<List<UserApplicator>>> GetUsers()
        {
            var queryTargets = _repo.GetUsers();
            var targets = await _commonDb.TolistAsync(queryTargets);
            return OperationHelper.GetElements(targets);
        }

        public async Task<ExtPostContainer<UserApplicator>> SaveEditUser(string id, string name, string rut, string email, string idJob, string[] idsRoles, string idNebulizer, string idTractor) {
            Job job = await _repoJob.GetJob(idJob);
            if (job == null)
                return OperationHelper.PostNotFoundElementException<UserApplicator>($"No se encontró el cargo con id {idJob}", idJob);
            List<Role> roles = new List<Role>();
            Role role;
            foreach (string idRole in idsRoles)
            {
                role = await _repoRole.GetRole(idRole);
                if (role == null)
                    return OperationHelper.PostNotFoundElementException<UserApplicator>($"No se encontró el rol con id {idRole}", idRole);
                roles.Add(role);
            }
            bool isApplicator = roles.Exists(r => r.Name.Equals("Aplicador"));
            Nebulizer nebulizer = null;
            Tractor tractor = null;
            if (isApplicator){
                if (!String.IsNullOrWhiteSpace(idNebulizer)){
                    nebulizer = await _repoNebulizer.GetNebulizer(idNebulizer);
                    if (nebulizer == null)
                        return OperationHelper.PostNotFoundElementException<UserApplicator>($"No se encontró la nebulizadora con id {idNebulizer}", idNebulizer);
                }
                if (!String.IsNullOrWhiteSpace(idTractor)){
                    tractor = await _repoTractor.GetTractor(idTractor);
                    if (tractor == null)
                        return OperationHelper.PostNotFoundElementException<UserApplicator>($"No se encontró el tractor con id {idTractor}", idTractor);
                }
            }
            var element = await _repo.GetUser(id);
            return await OperationHelper.EditElement(_commonDb, _repo.GetUsers(), id, element,
                s => {
                    s.Name = name;
                    s.Rut = rut;
                    s.Email = email;
                    s.Job = job;
                    s.Roles = roles;
                    s.Tractor = (isApplicator)?tractor:null;
                    s.Nebulizer = (isApplicator)?nebulizer:null;
                    return s;
                },
                _repo.CreateUpdateUser,
                $"No existe objetivo aplicación con id : {id}",
                s => (s.Rut.Equals(rut) && rut != element.Rut) || (s.Email.Equals(email) && email != element.Email),
                $"Este rut o correo ya existe"
            );
        }

        public async Task<ExtPostContainer<string>> SaveNewUser(string name, string rut, string email, string idJob, string[] idsRoles, string idNebulizer, string idTractor) {
            string objectId = await _graphApi.CreateUserIntoActiveDirectory(name, email);
            Job job = await _repoJob.GetJob(idJob);
            if (job == null)
                return OperationHelper.PostNotFoundElementException<string>($"No se encontró el cargo con id {idJob}", idJob);
            List<Role> roles = new List<Role>();
            Role role;
            foreach (string idRole in idsRoles)
            {
                role = await _repoRole.GetRole(idRole);
                if (role == null)
                    return OperationHelper.PostNotFoundElementException<string>($"No se encontró el rol con id {idRole}", idRole);
                roles.Add(role);
            }
            bool isApplicator = roles.Exists(r => r.Name.Equals("Aplicador"));
            Nebulizer nebulizer = null;
            Tractor tractor = null;
            if (isApplicator)
            {
                if (!String.IsNullOrWhiteSpace(idNebulizer))
                {
                    nebulizer = await _repoNebulizer.GetNebulizer(idNebulizer);
                    if (nebulizer == null)
                        return OperationHelper.PostNotFoundElementException<string>($"No se encontró la nebulizadora con id {idNebulizer}", idNebulizer);
                }
                if (!String.IsNullOrWhiteSpace(idTractor))
                {
                    tractor = await _repoTractor.GetTractor(idTractor);
                    if (tractor == null)
                        return OperationHelper.PostNotFoundElementException<string>($"No se encontró el tractor con id {idTractor}", idTractor);
                }
            }
            return await OperationHelper.CreateElement(_commonDb, _repo.GetUsers(),
                async s => await _repo.CreateUpdateUser(
                    new UserApplicator
                    {
                        Id = s,
                        ObjectIdAAD = objectId,
                        Name = name,
                        Rut = rut,
                        Email = email,
                        Job = job,
                        Roles = roles,
                        Nebulizer = (isApplicator) ? nebulizer : null,
                        Tractor = (isApplicator) ? tractor : null,
                    }),
                s => s.Rut.Equals(rut) || s.Email.Equals(email),
                $"Este nombre, rut o correo ya existe"
            );
        }
    }
}