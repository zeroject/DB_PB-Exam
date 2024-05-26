namespace Runner
{
    public class Runner
    {
        private readonly DbContext _dbContext;

        public Runner(DbContext dbContext)
        {
            _dbContext = dbContext;
            Non_SargableQuery._dbContext = dbContext;
            SargableQuery._dbContext = dbContext;
        }

        public async Task DDOSAttack(int Users, int times)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < Users; i++)
            {
                int userId = i; // Capture the loop variable
                tasks.Add(RunOnThreadAsync(() => SimulateUser(times)));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("DDOS attack completed");
        }

        private async Task RunOnThreadAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            var thread = new Thread(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            thread.Start();
            await tcs.Task;
        }

        private void SimulateUser(int operationsCount)
        {
            
            

            for (int i = 0; i < operationsCount; i++)
            {
                Non_SargableQuery.GetAllFromSpecificStreet();
                Non_SargableQuery.GetOrderDetailsFromCustomer();
                Non_SargableQuery.CreateNewCustomerWithOrderOfMilkAndBread();
                Non_SargableQuery.UpdatePriceOfItem();
                Non_SargableQuery.DeleteLastInsertCustomer();
                
                SargableQuery.GetAllFromSpecificStreet();
                SargableQuery.GetOrderDetailsFromCustomer();
                SargableQuery.CreateNewCustomerWithOrderOfMilkAndBread();
                SargableQuery.UpdatePriceOfItem();
                SargableQuery.DeleteLastInsertCustomer();
            }
        }
    }
}
