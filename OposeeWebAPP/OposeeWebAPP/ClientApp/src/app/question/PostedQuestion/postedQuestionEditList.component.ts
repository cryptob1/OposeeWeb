import { Component, OnInit } from '@angular/core';
import { first } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { User, LocalStorageUser, PostQuestionDetail } from '../../_models';
import { UserService } from '../../_services';
import { } from '../../_models/question';
import { ToastrService } from 'ngx-toastr';

@Component({ templateUrl: 'postedQuestionEditList.component.html', styleUrls: ['./postedQuestionEditList.component.css']  })
export class PostedQuestionEditList implements OnInit {
  currentUser: User;
  localStorageUser: LocalStorageUser;
  search: string ;
  //questionListing: QuestionListing[] = [];
  PostQuestionDetailList: PostQuestionDetail[] = [];
  isRecordLoaded: boolean = false;
  questionGetModel = { 'UserId': 0, 'Search': '', 'PageNumber': 0, 'TotalRecords': 0,'PageSize': 0 }

  private allItems: any[];

  // pager object
  pager: any = {};

  // paged items
  pagedItems: any[];

  constructor(private userService: UserService, private route: ActivatedRoute, private toastr: ToastrService,) {
    this.localStorageUser = JSON.parse(localStorage.getItem('currentUser'));



  }

  ngOnInit() {
    debugger;

    this.questionGetModel.Search = this.search;
    if (this.localStorageUser) {
      this.questionGetModel.UserId = this.localStorageUser.Id
    }
    else {
      this.questionGetModel.UserId = 0;
    }
    this.questionGetModel.PageNumber = 1;
    this.questionGetModel.TotalRecords = 5
    this.getQuestionListEdit(this.questionGetModel);


  }


  private getQuestionListEdit(questionGetModel) {
    debugger
    this.isRecordLoaded = false;

    this.userService.getQuestionListEditService(questionGetModel).subscribe(data => {
      debugger;
      if (data) {
        this.PostQuestionDetailList = data;
        this.questionGetModel.TotalRecords = data[0].TotalRecordcount
      }
      this.setPageonpageLoad(1, this.questionGetModel.TotalRecords)
      this.isRecordLoaded = true

    }, error => {
      this.isRecordLoaded = false;

    });
  }

  private getQuestionListEditPaging(questionGetModel) {
    debugger
    this.isRecordLoaded = false;
    this.userService.getQuestionListEditService(questionGetModel).subscribe(data => {
      debugger;
      if (data) {
        this.PostQuestionDetailList = data;
        this.questionGetModel.TotalRecords = data[0].TotalRecordcount
      }
      this.isRecordLoaded = true

    }, error => {
      this.isRecordLoaded = false;

    });
  }


  deleteQuestion(Id) {
    var questionGetModel = { 'Id': Id,'OwnerUserID':  this.questionGetModel.UserId }
    this.userService.deletePostQuestionwebService(questionGetModel)
      .pipe(first())
      .subscribe(data => {
        debugger;
        if (data) {
          this.toastr.success('record deleted', 'data save successfully.', { timeOut: 5000 });
          this.getQuestionListEditPaging(this.questionGetModel);

        }
        else {
          this.toastr.error('Wrong Record', 'some error occured.', { timeOut: 5000 });
        }
           

      },
        error => {
     
  
        });
    
  }

  
  PagingPagesload(PageNumber, PageSize) {
    debugger;

    this.questionGetModel.Search = this.search;
    this.questionGetModel.PageNumber = PageNumber;
    this.questionGetModel.PageSize = PageSize
    this.getQuestionListEditPaging(this.questionGetModel);



  }




  setPageonpageLoad(page, TotalRecords) {
    debugger;
    this.pager = this.getPager(TotalRecords, page);
  }

  setPage(page, TotalRecords) {
    debugger;
    this.pager = this.getPager(this.questionGetModel.TotalRecords, page);

    if (this.pager.totalPages >= page) {

      this.PagingPagesload(this.pager.currentPage, this.pager.pageSize);
    }
  }



  getPager(totalItems: number, currentPage: number = 1, pageSize: number = 10) {
    // calculate total pages

    let totalPages = Math.ceil(totalItems / pageSize);

    // ensure current page isn't out of range
    if (currentPage < 1) {
      currentPage = 1;
    } else if (currentPage > totalPages) {
      currentPage = totalPages;
    }

    let startPage: number, endPage: number;
    if (totalPages <= 10) {
      // less than 10 total pages so show all
      startPage = 1;
      endPage = totalPages;
    } else {
      // more than 10 total pages so calculate start and end pages
      if (currentPage <= 6) {
        startPage = 1;
        endPage = 10;
      } else if (currentPage + 4 >= totalPages) {
        startPage = totalPages - 9;
        endPage = totalPages;
      } else {
        startPage = currentPage - 5;
        endPage = currentPage + 4;
      }
    }

    // calculate start and end item indexes
    let startIndex = (currentPage - 1) * pageSize;
    let endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

    // create an array of pages to ng-repeat in the pager control
    let pages = Array.from(Array((endPage + 1) - startPage).keys()).map(i => startPage + i);

    // return object with all pager properties required by the view
    return {
      totalItems: totalItems,
      currentPage: currentPage,
      pageSize: pageSize,
      totalPages: totalPages,
      startPage: startPage,
      endPage: endPage,
      startIndex: startIndex,
      endIndex: endIndex,
      pages: pages
    };
  }




}
