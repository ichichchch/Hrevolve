import { request } from '../request';
import type { 
  TaxProfile, TaxRecord,
  PageRequest, PageResponse 
} from '@/types';

/** 报税管理 API */
export const taxApi = {
  // ==================== 税务档案 ====================
  
  /** 获取税务档案列表 */
  getTaxProfiles(params: PageRequest & { year?: number }) {
    return request.get<PageResponse<TaxProfile>>('/tax/profiles', { params });
  },
  
  /** 获取员工税务档案 */
  getEmployeeTaxProfile(employeeId: string, year: number) {
    return request.get<TaxProfile>(`/tax/profiles/employee/${employeeId}`, { params: { year } });
  },
  
  /** 创建/更新税务档案 */
  saveTaxProfile(data: Partial<TaxProfile>) {
    return request.post<TaxProfile>('/tax/profiles', data);
  },
  
  /** 批量导入税务档案 */
  importTaxProfiles(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return request.post<{ success: number; failed: number; errors: string[] }>(
      '/tax/profiles/import', 
      formData
    );
  },
  
  /** 删除税务档案 */
  deleteTaxProfile(id: string) {
    return request.delete(`/tax/profiles/${id}`);
  },
  
  // ==================== 税务计算 ====================
  
  /** 获取税务计算记录列表 */
  getTaxRecords(params: PageRequest & {
    periodId?: string;
    employeeId?: string;
    status?: string;
  }) {
    return request.get<PageResponse<TaxRecord>>('/tax/records', { params });
  },
  
  /** 获取税务计算记录详情 */
  getTaxRecord(id: string) {
    return request.get<TaxRecord>(`/tax/records/${id}`);
  },
  
  /** 计算个税 */
  calculateTax(data: { periodId: string; employeeIds?: string[] }) {
    return request.post<{ success: number; failed: number }>('/tax/calculate', data);
  },
  
  /** 重新计算个税 */
  recalculateTax(id: string) {
    return request.post<TaxRecord>(`/tax/records/${id}/recalculate`);
  },
  
  /** 标记已申报 */
  markDeclared(ids: string[]) {
    return request.post('/tax/records/declare', { ids });
  },
  
  /** 标记已缴纳 */
  markPaid(ids: string[]) {
    return request.post('/tax/records/pay', { ids });
  },
  
  // ==================== 税务报表 ====================
  
  /** 获取税务汇总报表 */
  getTaxSummaryReport(params: { year: number; month?: number }) {
    return request.get<{
      totalGrossIncome: number;
      totalTaxableIncome: number;
      totalDeductions: number;
      totalTaxAmount: number;
      employeeCount: number;
      byMonth: { month: string; taxAmount: number; employeeCount: number }[];
      byDepartment: { departmentName: string; taxAmount: number; employeeCount: number }[];
    }>('/tax/reports/summary', { params });
  },
  
  /** 获取个税明细报表 */
  getTaxDetailReport(params: { year: number; month: number }) {
    return request.get<{
      items: TaxRecord[];
      summary: {
        totalGrossIncome: number;
        totalTaxableIncome: number;
        totalDeductions: number;
        totalTaxAmount: number;
      };
    }>('/tax/reports/detail', { params });
  },
  
  /** 导出税务报表 */
  exportTaxReport(params: { year: number; month?: number; format: 'excel' | 'pdf' }) {
    return request.get('/tax/reports/export', { 
      params, 
      responseType: 'blob' 
    });
  },
  
  // ==================== 税务设置 ====================
  
  /** 获取税率表 */
  getTaxRates() {
    return request.get<{
      brackets: { min: number; max: number; rate: number; deduction: number }[];
      threshold: number;
      effectiveDate: string;
    }>('/tax/settings/rates');
  },
  
  /** 更新税率表 */
  updateTaxRates(data: {
    brackets: { min: number; max: number; rate: number; deduction: number }[];
    threshold: number;
    effectiveDate: string;
  }) {
    return request.put('/tax/settings/rates', data);
  },
  
  /** 更新税务档案 */
  updateTaxProfile(id: string, data: Partial<TaxProfile>) {
    return request.put<TaxProfile>(`/tax/profiles/${id}`, data);
  },
  
  /** 创建税务档案 */
  createTaxProfile(data: Partial<TaxProfile>) {
    return request.post<TaxProfile>('/tax/profiles', data);
  },
  
  /** 获取税务设置 */
  getTaxSettings() {
    return request.get<{
      taxYear: number;
      basicDeduction: number;
      socialInsuranceRate: number;
      housingFundRate: number;
      childEducation: number;
      continuingEducation: number;
      housingLoanInterest: number;
      housingRent: number;
      elderlySupport: number;
      infantCare: number;
    }>('/tax/settings');
  },
  
  /** 更新税务设置 */
  updateTaxSettings(data: Record<string, unknown>) {
    return request.put('/tax/settings', data);
  },
  
  /** 导出报税记录 */
  exportTaxRecords(params: { year: number; month?: number }) {
    return request.get('/tax/records/export', { params, responseType: 'blob' });
  },
};
