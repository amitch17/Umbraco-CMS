﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Models.Mapping;
using Umbraco.Web.Mvc;

using legacyUser = umbraco.BusinessLogic.User;
using System.Net.Http;
using System.Collections.Specialized;


namespace Umbraco.Web.Editors
{
    /// <summary>
    /// Controller to back the User.Resource service, used for fetching user data when already authenticated. user.service is currently used for handling authentication
    /// </summary>
    [PluginController("UmbracoApi")]
    public class UserController : UmbracoAuthorizedJsonController
    {
        /// <summary>
        /// Returns a user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserDetail GetById(int id)
        {
            var user = Services.UserService.GetUserById(id);
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Mapper.Map<UserDetail>(user);
        }

        /// <summary>
        /// Changes the users password
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage PostChangePassword(UserPasswordChange data)
        {   
         
            var u = UmbracoContext.Security.CurrentUser;
            if (!UmbracoContext.Security.ValidateBackOfficeCredentials(u.Username, data.OldPassword))
                return new HttpResponseMessage(HttpStatusCode.Forbidden);

            if(!UmbracoContext.Security.ChangePassword(data.OldPassword, data.NewPassword))
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Returns all active users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserBasic> GetAll()
        {
            return Services.UserService.GetAllUsers().Where(x => x.IsLockedOut == false).Select(Mapper.Map<UserBasic>);
        }
    }
}
