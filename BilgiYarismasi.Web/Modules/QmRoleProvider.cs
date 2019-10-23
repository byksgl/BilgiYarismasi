﻿using BilgiYarismasi.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BilgiYarismasi.Web.Modules
{
    public class QmRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            string[] roles;
            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                roles = ent.AspNetRoles.Select(p=>p.Name).ToArray();
            }
            return roles;
        }

        public override string[] GetRolesForUser(string username)
        {
            string[] roles = { };
            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                var user = ent.AspNetUsers.Where(p => p.UserName == username).FirstOrDefault();
                if (user == null) return roles;
                roles = user.AspNetRoles.Select(p => p.Name).ToArray();
            }
            return roles;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}