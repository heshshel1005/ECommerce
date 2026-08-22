# How the admin adds pages, blog, and FAQ

Content for the storefront (About, Privacy, Contact, Blog, FAQ) is managed with **ABP CMS Kit**. The storefront shows:

- **Pages** at `/page/{slug}` (e.g. `/page/faq`, `/page/about`) — one CMS “page” per URL slug.
- **Blog** at `/blog` — posts from the CMS blog (default blog slug: `default`).

Admins can manage this content in two ways.

---

## 1. From the admin UI (Angular)

When the **CMS** admin section is implemented:

1. Log in as an **admin** and open the side menu.
2. Go to **CMS → Pages** to:
   - List all pages.
   - **Add page**: set **Title**, **Slug**, and **Content** (HTML).  
     Examples: slug `faq` for FAQ, `about` for About, `privacy` for Privacy, `contact` for Contact.
   - Edit or delete existing pages.
3. Go to **CMS → Blogs** to:
   - List blogs and create a blog if needed (e.g. slug `default`).
   - Open a blog to see its **posts**.
   - **Add post**: set title, slug, short description, and content.  
     These posts appear on the storefront `/blog` page.

**FAQ** is just a normal page with slug **`faq`**. Create a page with slug `faq` and the desired title and content; the footer link “FAQ” already points to `/page/faq`.

---

## 2. Via the backend API (Swagger / Postman)

The backend exposes CMS Kit admin APIs. You can create and edit pages and blog posts without using the Angular admin:

1. Open **Swagger**: e.g. `https://localhost:44370/swagger` (or your host URL).
2. Authorize with an admin user (OAuth2 / OpenIddict as configured).
3. Use the **cms-kit-admin** endpoints, for example:
   - **Pages**
     - `GET /api/cms-kit-admin/pages` — list pages.
     - `POST /api/cms-kit-admin/pages` — create page (body: `title`, `slug`, `content`, etc.).
     - `GET /api/cms-kit-admin/pages/{id}` — get one page.
     - `PUT /api/cms-kit-admin/pages/{id}` — update page.
   - **Blogs**
     - `GET /api/cms-kit-admin/blogs` — list blogs.
     - `POST /api/cms-kit-admin/blogs` — create blog (e.g. name “Default”, slug `default`).
   - **Blog posts**
     - `GET /api/cms-kit-admin/blogs/blog-posts` — list posts (filter by blog).
     - `POST /api/cms-kit-admin/blogs/blog-posts` — create post (link to blog, set title, slug, content).

To add **FAQ**: create a page with slug **`faq`** and the desired title and HTML content. The storefront will show it at `/page/faq`.

---

## Summary

| Content   | How it’s stored        | Admin adds/edits it                    | Storefront URL   |
|----------|------------------------|----------------------------------------|------------------|
| FAQ      | CMS Kit **page**      | CMS → Pages (slug `faq`) or API        | `/page/faq`      |
| About    | CMS Kit **page**      | CMS → Pages (slug `about`) or API      | `/page/about`    |
| Blog     | CMS Kit **blog**      | CMS → Blogs (e.g. slug `default`)      | `/blog`          |
| Blog post| CMS Kit **blog post** | CMS → Blogs → [blog] → posts or API    | `/blog` (list)   |

The footer links (About, Privacy, Contact, Blog, FAQ) are already set to these routes; once the corresponding pages and blog exist in CMS Kit, they will show the content you create.
