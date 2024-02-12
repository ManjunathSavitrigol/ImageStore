using Image;
using ImageStore.Business.Interfaces;
using ImageStore.Data;
using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Business
{
    public class LoginBusiness : ILoginBusiness
    {
        //db instance
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly UserDetailsRepo _userDetails;

        public LoginBusiness()
        {
            _userDetails = new UserDetailsRepo(_unitOfWork);
        }

        public Response CheckUser(string email, string password)
        {
            Response res = new Response();
            SessionHelper sessionHelper = new SessionHelper();

            res.Message = " * ";
            try
            {
                //get encrypted token   
                var encryptedText = Helpers.Encrypt(password);


                var user = _userDetails.GetAll().Where(x => x.Email == email && x.Pass == encryptedText).FirstOrDefault();
                if (user != null)
                {
                    sessionHelper.SessionStart = "True";
                    sessionHelper.FullName = user.Full_Name;
                    sessionHelper.UserId = user.Id;
                    sessionHelper.UserType = user.UserType;
                    sessionHelper.LastLogin = user.Last_Login ?? DateTime.Now;
                    if (string.IsNullOrEmpty(user.Profile))
                        sessionHelper.ProfileImage = "~/Content/Assets/Images/javascript-logo-transparent-logo-javascript-images-3.png";  
                    else
                        sessionHelper.ProfileImage = user.Profile;  

                    //set token
                    Random random = new Random();
                    var token = Helpers.Encrypt(user.Id + "|" + DateTime.Now.ToString("yyyyMMddHHmmss") + "|" + random.Next(1000, 9999));
                    sessionHelper.Token = token;
                    user.Token = token;

                    switch (user.UserType)
                    {
                        case "A":
                            sessionHelper.Navigation = "_NavAdmin";
                            sessionHelper.Redirect = "Index*User";
                            break;
                        case "IU":
                            sessionHelper.Navigation = "_NavAdmin";
                            sessionHelper.Redirect = "Index*ImageUploader";
                            break;
                        default:
                            sessionHelper.Navigation = "_NavIA";
                            sessionHelper.Redirect = "Index*ImageApprove";
                            break;
                    }

                    user.Last_Login = DateTime.Now;
                    _userDetails.Update(user);

                    res.Object = sessionHelper;
                    res.Object1 = user;
                    res.Flag = true;

                }
                else
                {
                    res.Message = "error*Invalid username or password";
                    res.Flag = false;
                }
            }
            catch (Exception e) { }
            return res;
        }

        public Response RegisterUser(UserDetails userDetails)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                //check if Email is already registered
                int userCount = _userDetails.GetAll().Where(u => u.Email == userDetails.Email).Count();

                //register
                if (userCount == 0)
                {
                    User_Details newUser = new User_Details();
                    newUser.Email = userDetails.Email;
                    newUser.Full_Name = userDetails.FullName;
                    newUser.Last_Login = DateTime.Now;

                    //get encrypted password
                    string encryptPass = Helpers.Encrypt(userDetails.Password);
                    newUser.Pass = encryptPass;
                    newUser.UserType = "IU";
                    newUser.ActiveFlag = true;
                    _userDetails.Insert(newUser);

                    //get encrypted token
                    Random random = new Random();
                    string encryptToken = Helpers.Encrypt(newUser.Id + "|" + DateTime.Now.ToString("yyyyMMddHHmmss") + "|" + random.Next(1000, 9999));

                    newUser.Token = encryptToken;
                    _userDetails.Update(newUser);

                    res.Flag = true;
                }
                else
                {
                    res.Message = "warning*This email is already registered";
                }
            }
            catch (Exception e)
            {
                res.Message = "error*Failed to register";
            }
            return res;
        }

        public bool CheckToken(string token)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                User_Details user = _userDetails.SingleOrDefault(u => u.Token == token);
                if (user != null)
                {
                    return true;
                }
            }
            catch { }

            return false;
        }
    }
}
