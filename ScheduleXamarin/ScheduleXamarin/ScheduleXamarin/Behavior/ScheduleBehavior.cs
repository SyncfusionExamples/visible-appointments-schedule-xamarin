using Syncfusion.SfSchedule.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace ScheduleXamarin
{
    public class ScheduleBehavior : Behavior<ContentPage>
    {
        SfSchedule schedule;
        protected override void OnAttachedTo(ContentPage bindable)
        {
            base.OnAttachedTo(bindable);
            schedule = bindable.FindByName<SfSchedule>("schedule");
            var viewModel = bindable.BindingContext as SchedulerViewModel;

            schedule.VisibleDatesChangedEvent += OnVisibleDatesChangedEvent;

            var meeting = new Meeting();
            meeting.From = new DateTime(2020, 02, 26, 10, 0, 0);
            meeting.To = meeting.From.AddHours(1);
            meeting.EventName = "Occurs every alternate day";
            meeting.Color = Color.Green;

            var Meetings = new ObservableCollection<Meeting>();
            Meetings.Add(meeting);

            RecurrenceProperties recurrenceProperties = new RecurrenceProperties();
            recurrenceProperties.RecurrenceType = RecurrenceType.Weekly;
            recurrenceProperties.RecurrenceRange = RecurrenceRange.Count;
            recurrenceProperties.Interval = 1;
            recurrenceProperties.WeekDays = WeekDays.Monday | WeekDays.Wednesday | WeekDays.Friday;
            recurrenceProperties.RecurrenceCount = 10;
            meeting.RecurrenceRule = schedule.RRuleGenerator(recurrenceProperties, meeting.From, meeting.To);

            viewModel.Meetings.Add(meeting);
        }

        /// <summary>
        /// Gets visible appointments on visible date changed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVisibleDatesChangedEvent(object sender, VisibleDatesChangedEventArgs e)
        {
            List<Meeting> visibleAppointments;

            if (e.visibleDates.Count == 0)
                return;

            if (schedule.ScheduleView == ScheduleView.DayView)
            {
                visibleAppointments = this.GetVisibleAppointments(e.visibleDates.FirstOrDefault());
            }
            else
            {
                visibleAppointments = this.GetVisibleAppointments(e.visibleDates.First(), e.visibleDates.Last());
            }
        }

        /// <summary>
        /// Gets the appointments collection within the given date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private List<Meeting> GetVisibleAppointments(DateTime startDate, DateTime? endDate = null)
        {
            List<Meeting> appointments = new List<Meeting>();

            if (schedule.DataSource == null)
                return appointments;

            if (endDate == null)
            {
                foreach (Meeting app in schedule.DataSource)
                {
                    if (app.RecurrenceRule == null && (app.From.Date == startDate.Date || app.To.Date == startDate.Date))
                        appointments.Add(app);
                }
            }
            else
            {
                foreach (Meeting app in schedule.DataSource)
                {
                    if (app.RecurrenceRule == null && ((app.From.Date >= startDate.Date && app.From.Date <= endDate.Value.Date) ||
                        (app.To.Date >= startDate.Date && app.To.Date <= endDate.Value.Date)))
                        appointments.Add(app);
                }
            }

            //Gets the recurrence appointments within the date range
            foreach (Meeting app in schedule.DataSource)
            {
                if (app.RecurrenceRule != null)
                {
                    IEnumerable<DateTime> dateCollection = schedule.GetRecurrenceDateTimeCollection(app.RecurrenceRule, app.From);

                    if (endDate == null)
                    {
                        if (dateCollection.Any(d => d.Date == startDate.Date))
                        {
                            appointments.Add(app);
                        }

                    }
                    else
                    {
                        while (startDate <= endDate)
                        {
                            if (dateCollection.Any(d => d.Date == startDate.Date))
                            {
                                appointments.Add(app);
                            }
                            startDate = startDate.Date.AddDays(1);
                        }
                    }

                }
            }

            return appointments;
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            base.OnDetachingFrom(bindable);
            schedule = null;
            schedule.VisibleDatesChangedEvent -= OnVisibleDatesChangedEvent;
        }
    }
}
