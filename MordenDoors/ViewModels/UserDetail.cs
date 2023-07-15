using MordenDoors.Database;
using System.Linq;

namespace MordenDoors.ViewModels
{
    public class UserDetail
    {
        public static AspNetUsers Users(string email)
        {
            using (var context = new MordenDoorsEntities())
            {
                var res = context.AspNetUsers.Where(s => s.Email == email).SingleOrDefault();
                if (res == null)
                {
                    return new AspNetUsers();
                }
                else
                {
                    return res;
                }
            }
        }
    }
}