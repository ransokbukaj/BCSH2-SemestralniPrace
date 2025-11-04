
using DatabaseAccess;
using DatabaseAccess.Interface;

try
{
    LoadData.Accsess(ConnectionManager.GetConnection());

}
catch (Exception e)
{
    Console.WriteLine($"Error: {e.Message}");
}

