using Demo.Library.Extensions;
using Demo.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace Demo.Infrastructure
{
    /**
     * Simple Session State Manager derived from ISessionRepository
     */
    public class SessionRepository : ISessionRepository
    {
        /**
         * Identify the session key as a constant
         */
        const string SessionKey = "UserSessionKey";

        /**
         * Dependency for our class
         */
        ISession session;
        /**
         * Dependency Injection Provided by .net CORE
         */
        public SessionRepository(IHttpContextAccessor accessor)
        {
            this.session = accessor.HttpContext.Session;
        }

        /**
         * Custom Constructor to use at our AuthorizationAttribute
         */
        public SessionRepository(ISession session)
        {
            this.session = session;
        }

        /**
         * Gets and Sets user at session via SessionExtension
         */
        public User User {
            get => this.session.Get<User>(SessionKey);
            set => this.session.Set<User>(SessionKey, value);
        }

        /**
         * Destroys current Session
         */
        public void Destroy()
        {
            this.session.Remove(SessionKey);
        }
    }
}
