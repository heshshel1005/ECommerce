import type { IFormFile } from '../../microsoft/asp-net-core/http/models';
import type { ProductMediaType } from '../../catalog/product-media-type.enum';

export interface ProductMediaUploadRequest {
  productId?: string;
  file?: IFormFile;
  mediaType?: ProductMediaType;
  sortOrder?: number;
  isPrimary?: boolean;
  altText?: string | null;
}
