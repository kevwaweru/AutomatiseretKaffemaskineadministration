using CoffeeCrazy.Interfaces;
using CoffeeCrazy.Models;

namespace CoffeeCrazy.Services
{
    public class ScheduledJobService : BackgroundService
    {
        private readonly IJobRepo _jobRepo;
        private readonly ICRUDRepo<Machine> _machineRepo;

        public ScheduledJobService(IJobRepo jobRepo, ICRUDRepo<Machine> machineRepo)
        {
            _jobRepo = jobRepo;
            _machineRepo = machineRepo; 
        }

        /// <summary>
        /// Executes the background service to create jobs based on the Enum Frequencies (daily, weekly, monthly).
        /// Runs every day at 07:00 UTC and creates jobs for all machines.
        /// </summary>
        /// <param name="stoppingToken">Token used to cancel the background task.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;

                if (now.Hour == 07 && now.Minute == 00)
                {
                    await CreateJobsForTodayAsync(now);
                }


                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);


            }
        }

        /// <summary>
        /// Creates jobs for the current day based on job frequencies (daily, weekly, monthly).
        /// </summary>
        /// <param name="currentTime">The current UTC time.</param>
        private async Task CreateJobsForTodayAsync(DateTime currentTime)
        {
            var machines = await _machineRepo.GetAllAsync();

            if (currentTime.DayOfWeek != DayOfWeek.Saturday && currentTime.DayOfWeek != DayOfWeek.Sunday)
            {
                await CreateDailyJobsAsync(machines, currentTime);
            }

            if (currentTime.DayOfWeek == DayOfWeek.Monday)
            {
                await CreateWeeklyJobsAsync(machines, currentTime);
            }

            if (currentTime.Day == 1)
            {
                await CreateMonthlyJobsAsync(machines, currentTime);
            }
        }

        /// <summary>
        /// Creates daily jobs for all machines.
        /// </summary>
        /// <param name="machines">The list of machines.</param>
        /// <param name="currentTime">The current UTC time.</param>
        private async Task CreateDailyJobsAsync(IEnumerable<Machine> machines, DateTime currentTime)
        {
            foreach (var machine in machines)
            {
                // Fetch unique daily jobs by title
                var uniqueDailyJobs = await _jobRepo.GetGroupedJobsByFrequencyAsync(machine.MachineId, 1);

                foreach (var job in uniqueDailyJobs)
                {
                    var newJob = new Job
                    {
                        Title = job.Title,
                        Description = job.Description,
                        Comment = job.Comment,
                        IsCompleted = false,
                        DateCreated = currentTime,
                        Deadline = currentTime.AddHours(12),
                        FrequencyId = 1, 
                        MachineId = machine.MachineId,
                        UserId = job.UserId
                    };
                    await _jobRepo.CreateAsync(newJob);
                }
            }
        }

        /// <summary>
        /// Creates weekly jobs for all machines.
        /// </summary>
        /// <param name="machines">The list of machines.</param>
        /// <param name="currentTime">The current UTC time.</param>
        private async Task CreateWeeklyJobsAsync(IEnumerable<Machine> machines, DateTime currentTime)
        {
            foreach (var machine in machines)
            {
                // Fetch unique weekly jobs by title
                var uniqueWeeklyJobs = await _jobRepo.GetGroupedJobsByFrequencyAsync(machine.MachineId, 2);

                foreach (var job in uniqueWeeklyJobs)
                {
                    var newJob = new Job
                    {
                        Title = job.Title,
                        Description = job.Description,
                        Comment = job.Comment,
                        IsCompleted = false,
                        DateCreated = currentTime,
                        Deadline = currentTime.AddDays(7),
                        FrequencyId = 2,
                        MachineId = machine.MachineId,
                        UserId = job.UserId
                    };
                    await _jobRepo.CreateAsync(newJob);
                }
            }
        }

        /// <summary>
        /// Creates monthly jobs for all machines.
        /// </summary>
        /// <param name="machines">The list of machines.</param>
        /// <param name="currentTime">The current UTC time.</param>
        private async Task CreateMonthlyJobsAsync(IEnumerable<Machine> machines, DateTime currentTime)
        {
            foreach (var machine in machines)
            {
                // Fetch unique monthly jobs by title
                var uniqueMonthlyJobs = await _jobRepo.GetGroupedJobsByFrequencyAsync(machine.MachineId, 3);

                foreach (var job in uniqueMonthlyJobs)
                {
                    var newJob = new Job
                    {
                        Title = job.Title,
                        Description = job.Description,
                        Comment = job.Comment,
                        IsCompleted = false,
                        DateCreated = currentTime,
                        Deadline = currentTime.AddMonths(1), 
                        FrequencyId = 3, 
                        MachineId = machine.MachineId,
                        UserId = null
                    };
                    await _jobRepo.CreateAsync(newJob);
                }
            }
        }

    }
}
