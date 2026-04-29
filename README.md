Project Pluto: Carbon Footprint Tracker (CFT) – README
Overview

Project Pluto is a web-based Carbon Footprint Tracker (CFT) developed using ASP.NET Core Razor Pages and Entity Framework Core.

The application enables users to log daily carbon-emitting activities, monitor their environmental impact over time, and gain meaningful insights into their behaviour through structured analytics.

It fulfills our Project requirements by supporting:

Emission logging and tracking
Historical data management
Analytical summaries (daily, weekly, monthly, yearly)
Activity search and filtering
User account management
Environmental awareness through tips and news

The system is designed with a focus on simplicity, usability, and clear data presentation, ensuring users can easily understand and reflect on their carbon footprint.

Key Features:
User Authentication
Secure login, logout, signup, and password reset via Auth0
Automatic user creation and syncing using Auth0 ID
Username displayed in UI instead of email for improved user experience
Emission Logging
Data is stored per user and per date for integrity and privacy

Each emission entry includes:
Activity type (e.g., Transport, Energy)
Measurement (e.g., km, hours)
Carbon output (kg CO₂)

Additional functionality:
Dynamic search to quickly find activities
Prevents long scrolling lists and improves usability
Daily Tracking
Displays all emissions logged for the current day

Users can:
Add entries
Remove entries
View totals per activity
Historical Data
Navigate to previous days
View, edit, and delete past entries

The Profile page provides:
Total emissions logged
Total number of entries
Weekly, monthly and yearly summaries
Best (lowest emission) day
Highest emission category
Monthly totals for the past year
Highest emission category per month

Tips & News:
Environmental tips to encourage sustainable behaviour
FAQ section for guidance and general knowledge
Climate-related news articles to improve awareness and engagement

Admin Features:
Admin users can manage
Activities
Tips
News articles
Users


Directions for Use:

Setup:
Open Visual Studio
Restore NuGet packages
Update the database (if required)
Run the solution


Using the Application:

Unauthenticated users:
Access restricted to login page

Authenticated users can access:
Home
Emission Log
Tips
News
Profile
Settings

Admin Access:
Username: admin
Email: admin@sample.com
Password: Testdata1+


System Architecture:
The application is built as a single monolithic ASP.NET Core Razor Pages project rather than being split into multiple services or APIs.

Alternative Approaches Considered:
Separate APIs for emissions, tips, and news
Microservice-based architecture
Client-server split with frontend framework

Justification:
A single project architecture was chosen because:
It reduces overall system complexity
Simplifies development, debugging, and deployment
Keeps business logic centralised
Is more appropriate for an MVP-scale application

Technologies Used:
ASP.NET Core Razor Pages
Entity Framework Core
SQLite Database
Auth0 Authentication
Bootstrap (UI styling)

Core Entities:
User
Activity
ActivityType
EmissionEntry
Tips
NewsArticle
Settings / Notifications

Data Management:
All data is stored locally using SQLite and accessed through a single DbContext instance.
This ensures consistency, simplifies data access, and supports rapid development.


Key Problems & Design Decisions
User Identification (Auth0 Integration)
Issue:
Auth0 provides multiple identifiers (email, username, sub), which could lead to inconsistent or duplicate user records.

Solution:
Use the Auth0 unique identifier as the primary key for users.

Alternative Solutions:
Use email as the primary identifier
Use username as the identifier

Justification:
Emails and usernames can change over time, whereas the Auth0 ID is immutable and guaranteed to be unique, ensuring reliable user-data association.

Emission Logging Structure
Issue:
Users may struggle to find activities efficiently when large numbers of options are available.

Solution:
Implement a dynamic search-based activity selection system.

Alternative Solutions:
Static dropdown list
Categorised manual selection

Justification:
Search functionality improves usability, reduces time to input data, and scales effectively as more activities are added.

Daily-Based Data Separation
Architectural Decision:
Store and display emission entries grouped by day.

Alternative Solutions:
Store emissions as a continuous ungrouped list
Use separate tables for different time periods

Justification:
Daily grouping aligns with user behaviour and simplifies calculations for:
Weekly totals
Monthly summaries
Best-day analysis
Emission Analytics Placement

Architectural Decision:
Display all analytics on the Profile page.

Alternative Solutions:
Dedicated analytics dashboard page
Embedding analytics within the emission log page

Justification:
Centralising analytics avoids cluttering the main logging interface and provides a clear, consistent location for user insights.

Single Project Architecture
Architectural Decision:
Use a monolithic Razor Pages application.

Alternative Solutions:
Multi-API architecture
Microservices

Justification:
A monolithic approach simplifies development, reduces overhead, and is more suitable for a project of this scale.

Handling Missing Data
Issue:
Users may not have sufficient data to generate analytics.

Solution:
Use null-safe calculations
Display fallback messages such as “No data yet”

Alternative Solutions:
Restrict analytics until sufficient data exists
Display empty or incomplete visual components

Justification:
Providing fallback values improves user experience and prevents runtime errors.


Testing
A comprehensive test plan was conducted covering:
CRUD operations
Validation handling
Authentication and authorisation
Edge cases (invalid inputs, missing data)
Data integrity (per-user filtering)
Issues Resolved
Missing input validation
Unauthorised page access
Edge case handling for invalid inputs


Security

Authentication is fully managed by Auth0, including:
Login / Logout
Signup
Password management
Identity handling

Benefits:
Avoids insecure password storage
Eliminates custom authentication risks
Uses a trusted third-party identity provider

Limitations:
No automatic tracking (manual entry required)
No external API integration
Basic UI (functionality prioritised over design polish)
Limited data visualisation (no charts/graphs yet)

Future Improvements:
Data visualisation (charts and graphs)
Third-party API integration (e.g., energy tracking)
Goal setting and progress tracking
Notifications and reminders
Improved UI/UX and mobile responsiveness
Enhanced admin dashboard features
Extended Auth0 profile editing capabilities

