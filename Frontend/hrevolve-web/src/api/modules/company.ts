import { request } from '../request';
import type { 
  Tenant, CostCenter, Tag, ClockDevice, 
  PageRequest, PageResponse 
} from '@/types';

/** 公司设置 API */
export const companyApi = {
  // ==================== 公司信息 ====================
  
  /** 获取当前公司信息 */
  getTenant() {
    return request.get<Tenant>('/company/tenant');
  },
  
  /** 更新公司信息 */
  updateTenant(data: Partial<Tenant>) {
    return request.put<Tenant>('/company/tenant', data);
  },
  
  /** 上传公司Logo */
  uploadLogo(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return request.post<{ url: string }>('/company/tenant/logo', formData);
  },
  
  // ==================== 成本中心 ====================
  
  /** 获取成本中心列表 */
  getCostCenters(params?: PageRequest) {
    return request.get<PageResponse<CostCenter>>('/company/cost-centers', { params });
  },
  
  /** 获取成本中心树 */
  getCostCenterTree() {
    return request.get<CostCenter[]>('/company/cost-centers/tree');
  },
  
  /** 获取成本中心详情 */
  getCostCenter(id: string) {
    return request.get<CostCenter>(`/company/cost-centers/${id}`);
  },
  
  /** 创建成本中心 */
  createCostCenter(data: Partial<CostCenter>) {
    return request.post<CostCenter>('/company/cost-centers', data);
  },
  
  /** 更新成本中心 */
  updateCostCenter(id: string, data: Partial<CostCenter>) {
    return request.put<CostCenter>(`/company/cost-centers/${id}`, data);
  },
  
  /** 删除成本中心 */
  deleteCostCenter(id: string) {
    return request.delete(`/company/cost-centers/${id}`);
  },
  
  // ==================== 标签管理 ====================
  
  /** 获取标签列表 */
  getTags(params?: { category?: string }) {
    return request.get<Tag[]>('/company/tags', { params });
  },
  
  /** 创建标签 */
  createTag(data: Partial<Tag>) {
    return request.post<Tag>('/company/tags', data);
  },
  
  /** 更新标签 */
  updateTag(id: string, data: Partial<Tag>) {
    return request.put<Tag>(`/company/tags/${id}`, data);
  },
  
  /** 删除标签 */
  deleteTag(id: string) {
    return request.delete(`/company/tags/${id}`);
  },
  
  // ==================== 打卡设备 ====================
  
  /** 获取打卡设备列表 */
  getClockDevices(params?: PageRequest & { type?: string; isActive?: boolean }) {
    return request.get<PageResponse<ClockDevice>>('/company/clock-devices', { params });
  },
  
  /** 获取打卡设备详情 */
  getClockDevice(id: string) {
    return request.get<ClockDevice>(`/company/clock-devices/${id}`);
  },
  
  /** 创建打卡设备 */
  createClockDevice(data: Partial<ClockDevice>) {
    return request.post<ClockDevice>('/company/clock-devices', data);
  },
  
  /** 更新打卡设备 */
  updateClockDevice(id: string, data: Partial<ClockDevice>) {
    return request.put<ClockDevice>(`/company/clock-devices/${id}`, data);
  },
  
  /** 删除打卡设备 */
  deleteClockDevice(id: string) {
    return request.delete(`/company/clock-devices/${id}`);
  },
  
  /** 同步打卡设备数据 */
  syncClockDevice(id: string) {
    return request.post(`/company/clock-devices/${id}/sync`);
  },
};
