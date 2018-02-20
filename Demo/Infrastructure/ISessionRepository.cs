using Demo.Models.Entities;

namespace Demo.Infrastructure
{
    /**
     * This interface will provide the application to use a 
     * custom Session State Manager. So we can use it to inject
     * to our objects.
     */
    public interface ISessionRepository
    {
        /**
         * User Object
         */
        User User { get; set; }

        /**
         * Destroy method should end the Current User's Session
         */
        void Destroy();
    }
}
