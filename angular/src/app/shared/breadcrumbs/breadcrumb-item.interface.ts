/**
 * Single breadcrumb item. Use route for clickable links; omit for the current (last) page.
 */
export interface BreadcrumbItem {
  /** Display label or localization key (e.g. "ECommerce::Catalog"). */
  label: string;
  /** Router link for navigation; omit for current page. */
  route?: string | unknown[];
}
