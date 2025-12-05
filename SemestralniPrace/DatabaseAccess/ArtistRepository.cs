using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Data;

namespace DatabaseAccess
{
    public class ArtistRepository : IArtistRepository
    {
        public List<Artist> GetList()
        {
            return Test();
            //throw new NotImplementedException();
        }

        private List<Artist> testList = new List<Artist>() 
        {
        new Artist(1,"John","Doe",DateTime.Now,DateTime.Now,"Pokus s id 1"),
        new Artist(2,"Jane","Doe",DateTime.Now,DateTime.Now,"Pokus s id 2"),
        new Artist(3,"Martin","Doe",DateTime.Now,DateTime.Now,"Pokus s id 3"),
        new Artist(4,"Jack","Doe",DateTime.Now,DateTime.Now,"Pokus s id 4"),
        new Artist(5,"Johny","Doe",DateTime.Now,DateTime.Now,"Pokus s id 5")
        };

        private List<Artist> Test()
        {
            return testList;
        }

        public void SaveItem(Artist artist)
        {
            if (artist.Id == 0)
            {
                // insert
            }
            else
            {
                // update
            }
            throw new NotImplementedException();
        }

        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }
    }
}
