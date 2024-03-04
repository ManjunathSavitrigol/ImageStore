using Image;
using ImageStore.Business.Interfaces;
using ImageStore.Data;
using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure;
using ImageStore.Data.Infrastructure.Contract;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Business
{
    public class UserBusiness : IUserBusiness
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly UserDetailsRepo _userDetails;
        private readonly UserDetailsRepo _userRepo;
        private readonly LikesRepo _likesRepo;

        public UserBusiness()
        {
            _userDetails = new UserDetailsRepo(_unitOfWork);
            _userRepo = new UserDetailsRepo(_unitOfWork);
            _likesRepo = new LikesRepo(_unitOfWork);
        }

        public Response CreateUpdate(User_Details user)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                //check if the same email is registered or not
                int emailCount = _userDetails.Count(x => x.Email == user.Email && x.Id != user.Id);
                if (emailCount > 0)
                {
                    res.Message = "warning*This Email is already registered!";
                    res.Flag = false;
                    goto ret;
                }

                if (user.Id == 0)
                {
                    user.ActiveFlag = true;
                    _userDetails.Insert(user);
                    res.Message = "success*User added successfully!";
                    res.Flag = true;
                }
                else
                {
                    User_Details existingUser = _userDetails.SingleOrDefault(x => x.Id == user.Id);
                    if (existingUser != null)
                    {
                        existingUser.Full_Name = user.Full_Name;
                        existingUser.Email = user.Email;
                        existingUser.Pass = user.Pass;
                        existingUser.UserType = user.UserType;
                        existingUser.ActiveFlag = user.ActiveFlag;
                        _userDetails.Update(existingUser);

                        res.Message = "success*User updated successfully!";
                        res.Flag = true;

                    }
                    else
                    {
                        res.Message = "error*Not Found!";
                    }
                }


            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("CreateUpdate Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
                res.Message = "error*Something went wrong!";
            }
        ret:
            return res;
        }

        public Response Get(int id = 0, string search = "")
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                res.Object = _userDetails.GetAll().Where(x =>
                (id == 0 ? true : (x.Id == id))
                && ((string.IsNullOrEmpty(search) ? true : x.Full_Name.Contains(search))
                || (string.IsNullOrEmpty(search) ? true : x.Email.Contains(search)))
                && (x.UserType != "A")
                );

                res.Flag = true;
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Get Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }
            return res;
        }

        public Response GetWithLikes(int id)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                IEnumerable<User_Details> users = _userRepo.GetAll();
                IEnumerable<Likes> likes = _likesRepo.GetAll();

                res.Object = (from u in users
                              join l in likes
                              on u.Id equals l.UserId into ul
                              where u.Id == id
                              group ul by u.Id into groupul
                              select new UserDetails2
                              {
                                  //Id = groupul.Key,
                                  //Email = u.Email,
                                  //TotalDownloads = 
                              }

                              );
            }
            catch (Exception ex) { Helpers.WriteErrorLog("GetWithLike Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }
            return res;
        }

        public Response Deactivate(string ids)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                string[] splitIds = ids.Split(',');
                foreach (string id in splitIds)
                {
                    //try parse then continue
                    if (!int.TryParse(id, out int actId))
                        continue;

                    //get category and remove
                    User_Details user = _userDetails.SingleOrDefault(x => x.Id == actId);
                    if (user == null)
                        continue;
                    else
                    {
                        user.ActiveFlag = false;
                        _userDetails.Update(user);
                    }

                }

                res.Message = "success*User Updated Successfully!";
                res.Flag = true;
            }
            catch (Exception ex) { Helpers.WriteErrorLog("Deactivate Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }
            return res;
        }

        public Response Activate(string ids)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                string[] splitIds = ids.Split(',');
                foreach (string id in splitIds)
                {
                    //try parse then continue
                    if (!int.TryParse(id, out int actId))
                        continue;

                    //get category and remove
                    User_Details user = _userDetails.SingleOrDefault(x => x.Id == actId);
                    if (user == null)
                        continue;
                    else
                    {
                        user.ActiveFlag = true;
                        _userDetails.Update(user);
                    }

                }

                res.Message = "success*User Updated Successfully!";
                res.Flag = true;
            }
            catch (Exception ex) { Helpers.WriteErrorLog("Activate Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }
            return res;
        }

        public Response ChangePassword(int userid, string oldpass, string newpass)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                //get the user
                User_Details user = _userDetails.SingleOrDefault(x => x.Id == userid);
                if (user == null)
                {
                    res.Message = "error*User not found!";
                    goto ret;
                }

                string encryptedoldpass = Helpers.Encrypt(oldpass);
                if (encryptedoldpass != user.Pass)
                {
                    res.Message = "error*Wrong old password!";
                    goto ret;
                }

                user.Pass = Helpers.Encrypt(newpass);
                _userDetails.Update(user);
                res.Flag = true;
                res.Message = "success*Password updated successfully!";
            }
            catch (Exception ex) { Helpers.WriteErrorLog("ChangePassword Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }

        ret:
            return res;
        }
        //private string CreatePassword()
        //{
        //    string newPassword;
        //    try
        //    {
        //        int length = 0;
        //        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        //        StringBuilder res = new StringBuilder();
        //        Random rnd = new Random();
        //        while (length < 10)
        //        {
        //            res.Append(valid[rnd.Next(valid.Length)]);
        //            length++;

        //        }

        //        return res.ToString();

        //    }
        //    catch { newPassword = "Manju@123"; }
        //    return newPassword;
        //}
    }
}
