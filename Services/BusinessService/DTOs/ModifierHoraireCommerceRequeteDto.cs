namespace BusinessService.DTOs
{
    public class ModifierHoraireCommerceRequeteDto
    {
        public DayOfWeek JourSemaine { get; set; }

        public TimeSpan HeureOuverture { get; set; }

        public TimeSpan HeureFermeture { get; set; }

        public bool EstFerme { get; set; }
    }
}