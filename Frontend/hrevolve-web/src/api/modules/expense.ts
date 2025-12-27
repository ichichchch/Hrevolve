import { request } from '../request';
import type { 
  ExpenseType, ExpenseRequest, ExpenseItem,
  PageRequest, PageResponse 
} from '@/types';

/** 报销管理 API */
export const expenseApi = {
  // ==================== 报销类型 ====================
  
  /** 获取报销类型列表 */
  getExpenseTypes(params?: { category?: string; isActive?: boolean }) {
    return request.get<ExpenseType[]>('/expenses/types', { params });
  },
  
  /** 获取报销类型详情 */
  getExpenseType(id: string) {
    return request.get<ExpenseType>(`/expenses/types/${id}`);
  },
  
  /** 创建报销类型 */
  createExpenseType(data: Partial<ExpenseType>) {
    return request.post<ExpenseType>('/expenses/types', data);
  },
  
  /** 更新报销类型 */
  updateExpenseType(id: string, data: Partial<ExpenseType>) {
    return request.put<ExpenseType>(`/expenses/types/${id}`, data);
  },
  
  /** 删除报销类型 */
  deleteExpenseType(id: string) {
    return request.delete(`/expenses/types/${id}`);
  },
  
  // ==================== 报销申请 ====================
  
  /** 获取报销申请列表 */
  getExpenseRequests(params: PageRequest & {
    employeeId?: string;
    status?: string;
    category?: string;
    startDate?: string;
    endDate?: string;
  }) {
    return request.get<PageResponse<ExpenseRequest>>('/expenses/requests', { params });
  },
  
  /** 获取我的报销申请 */
  getMyExpenseRequests(params: PageRequest & { status?: string }) {
    return request.get<PageResponse<ExpenseRequest>>('/expenses/requests/my', { params });
  },
  
  /** 获取待审批的报销申请 */
  getPendingExpenseRequests(params?: PageRequest) {
    return request.get<PageResponse<ExpenseRequest>>('/expenses/requests/pending', { params });
  },
  
  /** 获取报销申请详情 */
  getExpenseRequest(id: string) {
    return request.get<ExpenseRequest>(`/expenses/requests/${id}`);
  },
  
  /** 创建报销申请 */
  createExpenseRequest(data: {
    expenseTypeId: string;
    amount: number;
    currency?: string;
    expenseDate: string;
    description: string;
    items?: Partial<ExpenseItem>[];
  }) {
    return request.post<ExpenseRequest>('/expenses/requests', data);
  },
  
  /** 更新报销申请 */
  updateExpenseRequest(id: string, data: Partial<ExpenseRequest>) {
    return request.put<ExpenseRequest>(`/expenses/requests/${id}`, data);
  },
  
  /** 提交报销申请 */
  submitExpenseRequest(id: string) {
    return request.post(`/expenses/requests/${id}/submit`);
  },
  
  /** 撤回报销申请 */
  cancelExpenseRequest(id: string) {
    return request.post(`/expenses/requests/${id}/cancel`);
  },
  
  /** 审批报销申请 */
  approveExpenseRequest(id: string, data: { approved: boolean; comment?: string }) {
    return request.post(`/expenses/requests/${id}/approve`, data);
  },
  
  /** 标记报销已支付 */
  markExpensePaid(id: string, data: { paymentMethod: string; paidAt: string }) {
    return request.post(`/expenses/requests/${id}/pay`, data);
  },
  
  /** 删除报销申请 */
  deleteExpenseRequest(id: string) {
    return request.delete(`/expenses/requests/${id}`);
  },
  
  /** 上传报销凭证 */
  uploadReceipt(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return request.post<{ url: string }>('/expenses/receipts/upload', formData);
  },
  
  // ==================== 报销统计 ====================
  
  /** 获取报销统计 */
  getExpenseStats(params: { year: number; month?: number; departmentId?: string }) {
    return request.get<{
      totalAmount: number;
      approvedAmount: number;
      pendingAmount: number;
      rejectedAmount: number;
      byCategory: { category: string; amount: number }[];
      byDepartment: { departmentName: string; amount: number }[];
      byMonth: { month: string; amount: number }[];
    }>('/expenses/stats', { params });
  },
};
