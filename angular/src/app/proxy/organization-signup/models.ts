import type { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { OrganizationBusinessType } from '../organizations/organization-business-type.enum';
import type { OrganizationSignupStatus } from '../organizations/organization-signup-status.enum';

export interface OrganizationSignupLogoUploadDto {
  uploadSessionId?: string;
  relativePath?: string;
}

export interface OrganizationSignupRequestDto extends EntityDto<string> {
  tenantName?: string;
  displayName?: string;
  legalName?: string | null;
  businessType?: OrganizationBusinessType;
  website?: string | null;
  phone?: string | null;
  shortDescription?: string | null;
  logoFilePathOrKey?: string | null;
  adminEmail?: string;
  adminUserName?: string;
  adminDisplayName?: string;
  status?: OrganizationSignupStatus;
  rejectionReason?: string | null;
  reviewedTime?: string | null;
  reviewerUserId?: string | null;
  createdTenantId?: string | null;
  creationTime?: string;
}

export interface OrganizationSignupRequestListRequestDto extends PagedAndSortedResultRequestDto {
  status?: OrganizationSignupStatus | null;
}

export interface OrganizationSignupSubmitDto {
  tenantName: string;
  displayName: string;
  legalName?: string | null;
  businessType: OrganizationBusinessType;
  website?: string | null;
  phone?: string | null;
  shortDescription?: string | null;
  logoUploadSessionId?: string | null;
  logoRelativePath?: string | null;
  adminEmail: string;
  adminUserName: string;
  adminDisplayName: string;
  adminPassword: string;
}

export interface OrganizationSignupSubmitResultDto {
  requestId?: string;
  message?: string;
}

export interface RejectOrganizationSignupDto {
  reason: string;
}

export interface RepairTenantAdminPermissionsResultDto {
  totalTenants?: number;
  repairedTenants?: number;
  failedTenants?: number;
}
