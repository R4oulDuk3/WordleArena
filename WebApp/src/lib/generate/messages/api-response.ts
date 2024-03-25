/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { ApiResponseStatusCode } from "./api-response-status-code";

export interface ApiResponse<T> {
    statusCode: ApiResponseStatusCode;
    data: T;
    message: string;
}
