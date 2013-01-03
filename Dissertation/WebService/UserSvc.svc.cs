﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WebService {
    public class UserSvc : IUserSvc {

        public BusinessLayer.User AddUser(string forename, string surname, string username, string password) {
            return BusinessLayer.User.AddUser(forename, surname, username, password);
        }

        public void ModifyUser(String authUsername, string authPassword, int userId, string forename, string surname, string password) {

            AuthUser(authUsername, authPassword, userId);

            BusinessLayer.User u = BusinessLayer.User.Populate(userId);

            u.Forename = forename;
            u.Password = password;
            u.Surname = surname;

            u.Save();
        }

        public BusinessLayer.UserDevice AddUserDevice(string authUsername, String authPassword, string deviceType, int deviceMemoryResource, int deviceProcRating) {

            AuthUser(authUsername, authPassword);

            return BusinessLayer.UserDevice.AddUserDevice(authUsername, deviceType, deviceMemoryResource, deviceProcRating);
        }

        public void DeleteUserDevice(string authUsername, String authPassword, int deviceId) {
            BusinessLayer.UserDevice ud = BusinessLayer.UserDevice.Populate(deviceId);

            if (ud.User.Username.Equals(authUsername)) {
                ud.Delete();
            }
        }

        private void AuthUser(String authUsername, string authPassword, int userId = -1) {
            if (BusinessLayer.User.AuthenticateUser(authUsername, authPassword)) {
                if (userId != -1) {
                    if (BusinessLayer.User.Populate(authUsername).UserId != userId) {
                        throw new Exception("You many only modify your own user");
                    } else {
                        throw new Exception("Authentication required");
                    }
                } else {
                    
                }
            } else {
                throw new Exception("Authentication required");
            }
        }

    }


}
