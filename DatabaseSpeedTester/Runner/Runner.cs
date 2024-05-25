namespace Runner
{
    public class Runner
    {
        private readonly DbContext _dbContext;

        public Runner(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DDOSAttack(int Users, int times)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < Users; i++)
            {
                int userId = i; // Capture the loop variable
                tasks.Add(RunOnThreadAsync(() => SimulateUser(times, userId)));
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

        private void SimulateUser(int operationsCount, int User)
        {
            Non_SargableQuery nonSargableQuery = new Non_SargableQuery(_dbContext);
            SargableQuery sargableQuery = new SargableQuery(_dbContext);

            for (int i = 0; i < operationsCount; i++)
            {
                nonSargableQuery.GetAllFromSpecificStreet();
                nonSargableQuery.GetOrderDetailsFromCustomer();
                nonSargableQuery.CreateNewCustomerWithOrderOfMilkAndBread();
                nonSargableQuery.UpdatePriceOfItem();
                nonSargableQuery.DeleteLastInsertCustomer();

                sargableQuery.GetAllFromSpecificStreet();
                sargableQuery.GetOrderDetailsFromCustomer();
                sargableQuery.CreateNewCustomerWithOrderOfMilkAndBread();
                sargableQuery.UpdatePriceOfItem();
                sargableQuery.DeleteLastInsertCustomer();
            }
        }
    }
}
