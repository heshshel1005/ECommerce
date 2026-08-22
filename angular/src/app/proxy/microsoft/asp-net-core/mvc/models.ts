import type { EntityTagHeaderValue } from '../../net/http/headers/models';

export interface ActionResult {
}

export interface FileContentResult extends FileResult {
  fileContents?: number[];
}

export interface FileResult extends ActionResult {
  contentType?: string;
  fileDownloadName?: string;
  lastModified?: string | null;
  entityTag?: EntityTagHeaderValue | null;
  enableRangeProcessing?: boolean;
}

export interface IActionResult {
}
