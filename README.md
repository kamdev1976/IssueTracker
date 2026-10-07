# Issue Tracker Web Application (ASP.NET Web Forms)

A modernized **ASP.NET Web Forms** application built with an N-Tier architecture, utilizing **Entity Framework**, **Repository & Unit of Work Patterns**, **Autofac Dependency Injection**, and a responsive **Bootstrap 5 UI**.

---

## 🏗️ Project Architecture

The solution is divided into three main projects:

1. **`IssueTracker.Core`**:
   - Contains domain entities (`Issue`, etc.).
   - Contains repository and Unit of Work interfaces (`IRepository<T>`, `IUnitOfWork`).

2. **`IssueTracker.Data`**:
   - Manages single-database context persistence using a soft-delete (`IsDeleted`) design pattern.
   - Implements data persistence using the Generic Repository and Unit of Work patterns.
   - **Centralized Validation & Transaction Control**: `Repository<T>` handles deferred execution via `IQueryable`, while `UnitOfWork.Complete()` executes atomic commits and logs formatted `DbEntityValidationException` details.

3. **`IssueTracker.Web`**:
   - Presentation layer containing primary tracker (`Default.aspx`) and archive tracker (`Archive.aspx`).
   - Styled with Bootstrap 5.
   - Configured with **Autofac** for constructor and property dependency injection of `IUnitOfWork` into ASP.NET pages.

---

## ✨ Key Features & Architectural Enhancements

- **Single-Database Soft-Delete Architecture**: Streamlined single-database design using `IsDeleted` flags, eliminating dual-database synchronization issues and supporting complete record restoration from the archive view back to active status.
- **Advanced Search & Filtering**: 
  - **3-Character Threshold**: Live searching automatically triggers once a minimum of 3 characters is entered.
  - **400ms Debounce**: Optimized with a 400ms debounce timer to prevent excessive server requests while typing.
  - **Cursor & Focus Retention**: Preserves focus and cursor position during asynchronous partial postbacks using `PageRequestManager` and hidden tracking fields.
- **Database-Level Pagination**: High-performance pagination leveraging SQL `Skip` and `Take` combined with `VirtualItemCount` and custom paging (`AllowCustomPaging="True"`).
- **Performance Optimization (`.AsNoTracking`)**: Read-only grid queries utilize `.AsNoTracking()` to reduce memory consumption and speed up database execution.
- **Dependency Injection**: **Autofac** manages component lifecycles to prevent connection leaks across page requests.
- **Centralized Exception Handling**: Entity Framework validation errors (`DbEntityValidationException`) are captured and formatted down to specific property names inside `UnitOfWork.Complete()`.
- **Modern UI**: Mobile-friendly, responsive interface built with Bootstrap 5 and ASP.NET `UpdatePanel` controls for flicker-free grid updates.

---

## 🛠️ Tech Stack & Dependencies

- **Framework**: .NET Framework 4.8 / ASP.NET Web Forms
- **ORM**: Entity Framework 6.5.2 (Code First / EDMX)
- **Database**: SQL Server LocalDB (`App_Data/*.mdf`)
- **IoC Container**: Autofac (migrated from legacy Unity)
- **UI Framework**: Bootstrap 5

---

## 💻 Local Setup & Execution Guide (Step-by-Step for Code Reviewers)

Follow these steps to set up and run the application on any development machine:

### Prerequisites
- **Visual Studio 2019 or Visual Studio 2022** with the **ASP.NET and web development** workload installed.
- **SQL Server Express LocalDB** installed (included by default with Visual Studio setup as `(LocalDB)\MSSQLLocalDB`).

### Setup Steps
1. **Clone the repository**:
   ```bash
   git clone [https://github.com/kamdev1976/IssueTracker.git](https://github.com/kamdev1976/IssueTracker.git)
