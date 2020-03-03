# How to get visible appointments in Xamarin.Forms Schedule (SfSchedule) ?

You can get the visible custom appointments with recurrence appointments of Xamarin.Forms SfSchedule in [VisibleDatesChangedEvent](https://help.syncfusion.com/cr/cref_files/xamarin/Syncfusion.SfSchedule.XForms~Syncfusion.SfSchedule.XForms.SfSchedule~VisibleDatesChangedEvent_EV.html).

Initialize an event handler for the **VisibleDatesChangedEvent** of SfSchedule to get the visible appointments on visible dates changed.
``` c#
schedule.VisibleDatesChangedEvent += OnVisibleDatesChangedEvent;

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
```
Get the visible appointments by comparing the visible dates and the date of each appointment in Schedule DataSource. 
``` c#
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
    return appointments;
}
```
You can also check the [RRule](https://help.syncfusion.com/cr/cref_files/xamarin/Syncfusion.SfSchedule.XForms~Syncfusion.SfSchedule.XForms.RecurrenceProperties~RecurrenceRule.html) and get the RecurrenceAppointments using [GetRecurrenceDateTimeCollection](https://help.syncfusion.com/cr/cref_files/xamarin/Syncfusion.SfSchedule.XForms~Syncfusion.SfSchedule.XForms.SfSchedule~GetRecurrenceDateTimeCollection.html) method. Check the date collection with the visible date range to get the visible appointments.
``` c#
private List<Meeting> GetVisibleAppointments(DateTime startDate, DateTime? endDate = null)
{
    List<Meeting> appointments = new List<Meeting>();

    if (schedule.DataSource == null)
        return appointments;
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
```
